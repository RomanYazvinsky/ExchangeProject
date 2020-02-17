import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {BehaviorSubject, EMPTY, Observable} from 'rxjs';
import {async} from 'rxjs/internal/scheduler/async';
import {filter, finalize, first, map, observeOn, switchMap, tap} from 'rxjs/operators';
import {AuthInfo} from '../models/auth-info';
import {UserDto} from '../models/user.dto';

@Injectable()
export class AuthService {
  private static readonly MaxTimeDiffThreshold: number = Date.UTC(1, 1, 1, 1, 5) - Date.UTC(1, 1, 1, 1, 0);
  private _isCheckAccessExpired: boolean | null = null;
  private readonly _authentication$: BehaviorSubject<AuthInfo | null>
    = new BehaviorSubject<AuthInfo | null>(null);
  private readonly _currentUser$: BehaviorSubject<UserDto | null>
    = new BehaviorSubject<UserDto | null>(null);
  private _isAuthBlocked: boolean = false;

  private get checkAccessValidationExpiration(): boolean {
    if (this._isCheckAccessExpired !== null) {
      return this._isCheckAccessExpired;
    }
    this._isCheckAccessExpired = localStorage.getItem('checkAccessValidationTime') === 'true';
    return this._isCheckAccessExpired;
  }

  private setAuthInfo(value: AuthInfo | null): void {
    if (!value) {
      localStorage.removeItem('authInfo');
    } else {
      localStorage.setItem('authInfo', JSON.stringify(value));
    }
    this._authentication$.next(value);
  }

  private setCheckAccessExpired(value: boolean): void {
    this._isCheckAccessExpired = value;
    localStorage.setItem('checkAccessValidationTime', String(value));
  }

  private loadAuthInfo() {
    const authInfo: AuthInfo = JSON.parse(localStorage.getItem('authInfo'));
    this._authentication$.next(authInfo);
  }

  private loadCurrentUser(): void {
    this.authentication$.pipe(
      observeOn(async), // runs asynchronous
      first(),
      switchMap(auth => {
        return !!auth
          ? this.http.get<UserDto | null>('/api/currentUser')
          : EMPTY;
      })).subscribe(user => this.setCurrentUser(user));
  }

  private setCurrentUser(user: UserDto | null) {
    this._currentUser$.next(user);
  }

  private updateAuth(authInfo: AuthInfo | null) {
    this.setAuthInfo(authInfo);
    if (!authInfo) {
      this.setCheckAccessExpired(false);
      this.setCurrentUser(null);
      return;
    }
    const serverUtcNow: Date = new Date(authInfo.serverUtcNow);
    this._isCheckAccessExpired = Math.abs(serverUtcNow.getTime() - Date.now()) < AuthService.MaxTimeDiffThreshold;
    this.setCheckAccessExpired(this._isCheckAccessExpired);
    this.setCurrentUser(authInfo.userInfo);
  }

  constructor(private http: HttpClient) {
    this.loadAuthInfo();
    this.loadCurrentUser();
  }

  login(username: string, password: string): Observable<AuthInfo> {
    return this.http.post('/api/login', {
        username,
        password
      }, {observe: 'body'}
    ).pipe(
      map(token => token as AuthInfo),
      filter(token => !!token),
      tap(token => this.updateAuth(token))
    );
  }

  logout(): Observable<void> {
    const auth = this.currentAuthentication;
    return this.http.get<void>('/api/logout', {
        params: {
          refreshToken: auth.refreshToken
        }
      }
    ).pipe(tap(() => {
      this.setCheckAccessExpired(false);
      this.setAuthInfo(null);
      this.setCurrentUser(null);
    }));
  }

  refreshTokens(): Observable<AuthInfo> {
    const auth = this.currentAuthentication;
    this._isAuthBlocked = true;
    return this.http.get<AuthInfo | null>('/api/refreshToken', {
      params: {refreshToken: auth.refreshToken}
    }).pipe(
      filter(token => !!token),
      tap(token => this.updateAuth(token)),
      finalize(() => this._isAuthBlocked = false)
    );
  }

  get authentication$(): Observable<AuthInfo | null> {
    return this._authentication$;
  }

  get currentAuthentication(): AuthInfo | null {
    return this._authentication$.getValue();
  }

  get currentUser$(): Observable<UserDto | null> {
    return this._currentUser$;
  }

  isTokenExpired(): boolean {
    const authInfo = this._authentication$.getValue();
    const checkDate = this.checkAccessValidationExpiration;
    if (!authInfo || !checkDate) {
      return false;
    }
    const currentTime = Date.now();
    const accessUtcValidTo: Date = new Date(authInfo.accessUtcValidTo);
    return !authInfo.accessToken || currentTime > accessUtcValidTo.getTime();
  }

  isAuthPossible(): boolean {
    const authInfo = this._authentication$.getValue();
    return !!authInfo && !!authInfo.accessToken && !this._isAuthBlocked;
  }
}

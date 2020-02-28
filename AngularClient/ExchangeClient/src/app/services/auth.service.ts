import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {BehaviorSubject, EMPTY, Observable, of} from 'rxjs';
import {catchError, filter, finalize, first, map, switchMap, tap} from 'rxjs/operators';
import {AuthDto} from '../models/auth-dto';
import {User} from '../models/user';
import {Base64Converter} from '../Utils/base64.converter';

@Injectable()
export class AuthService {
  private readonly _authentication$: BehaviorSubject<AuthDto | null>
    = new BehaviorSubject<AuthDto | null>(null);
  private readonly _currentUser$: BehaviorSubject<User | null>
    = new BehaviorSubject<User | null>(null);
  private _isAuthBlocked: boolean = false;
  private _expiration: Date | null = null;

  private setAuthInfo(value: AuthDto | null): void {
    if (!value) {
      localStorage.removeItem('authInfo');
      this._expiration = null;
    } else {
      this.setAccessExpiration(value.accessToken);
      localStorage.setItem('authInfo', JSON.stringify(value));
    }
    this._authentication$.next(value);
  }

  private loadAuthInfo(): void {
    const authInfo: AuthDto | null = JSON.parse(localStorage.getItem('authInfo'));
    this.setAccessExpiration(authInfo?.accessToken);
    this._authentication$.next(authInfo);
  }

  private setAccessExpiration(accessToken: string): void {
    if (!accessToken) {
      return;
    }
    const base64 = accessToken.split('.')[1];
    if (!base64) {
      return;
    }
    const token = Base64Converter.convert(base64);
    const expirationDate = new Date(token.exp as number * 1000);
    if (expirationDate.getTime() < Date.now()) {
      this._expiration = null;
      return;
    }
    this._expiration = expirationDate;
  }

  private loadCurrentUser(): void {
    setTimeout(() => {
      this.authentication$.pipe(
        first(),
        switchMap(auth => {
          return !!auth
            ? this.http.get<User | null>('/api/currentUser')
            : EMPTY;
        })).subscribe(user => this.setCurrentUser(user));
    });
  }

  private setCurrentUser(user: User | null) {
    this._currentUser$.next(user);
  }

  private updateAuth(authInfo: AuthDto | null) {
    this.setAuthInfo(authInfo);
    if (!authInfo) {
      this.setCurrentUser(null);
      return;
    }
    this.http.get<User | null>('/api/currentUser').subscribe(user => this.setCurrentUser(user))
  }

  constructor(private http: HttpClient, private router: Router) {
    this.loadAuthInfo();
    this.loadCurrentUser();
  }

  login(username: string, password: string): Observable<AuthDto> {
    return this.http.post('/api/login', {
        username,
        password
      }, {observe: 'body'}
    ).pipe(
      map(token => token as AuthDto),
      filter(token => !!token),
      tap(token => this.updateAuth(token))
    );
  }

  logout(): Observable<void> {
    return of(null).pipe(tap(() => {
      this.setAuthInfo(null);
      this.setCurrentUser(null);
    }));
  }

  refreshTokens(): Observable<AuthDto> {
    const auth = this.currentAuthentication;
    this._isAuthBlocked = true;
    return this.http.get<AuthDto | null>('/api/refreshToken', {
      params: {refreshToken: auth.refreshToken}
    }).pipe(
      catchError(() => this.logout().pipe(switchMap(() => {
        this.router.navigate(['login']);
        return EMPTY;
      }))),
      filter(token => !!token),
      tap(token => this.updateAuth(token)),
      finalize(() => this._isAuthBlocked = false)
    );
  }

  get authentication$(): Observable<AuthDto | null> {
    return this._authentication$;
  }

  get currentAuthentication(): AuthDto | null {
    return this._authentication$.getValue();
  }

  get currentUser$(): Observable<User | null> {
    return this._currentUser$;
  }

  isTokenExpired(): boolean {
    if (!this._expiration) {
      return false;
    }
    const currentTime = Date.now();
    return currentTime > this._expiration.getTime();
  }

  isAuthPossible(): boolean {
    const authInfo = this._authentication$.getValue();
    return !!authInfo && !!authInfo.accessToken && !this._isAuthBlocked;
  }
}

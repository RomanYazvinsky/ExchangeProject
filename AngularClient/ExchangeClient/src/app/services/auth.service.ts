﻿import {HttpClient} from '@angular/common/http';
import {Injectable, Injector} from '@angular/core';
import {Router} from '@angular/router';
import {BehaviorSubject, EMPTY, Observable, of} from 'rxjs';
import {async} from 'rxjs/internal/scheduler/async';
import {catchError, filter, finalize, first, map, mergeMap, observeOn, switchMap, tap} from 'rxjs/operators';
import {AuthDto} from '../models/auth-dto';
import {UserDto} from '../models/user.dto';
import {Base64Converter} from '../Utils/base64.converter';

@Injectable()
export class AuthService {
  private readonly _authentication$: BehaviorSubject<AuthDto | null>
    = new BehaviorSubject<AuthDto | null>(null);
  private readonly _currentUser$: BehaviorSubject<UserDto | null>
    = new BehaviorSubject<UserDto | null>(null);
  private _isAuthBlocked: boolean = false;
  private _expiration: Date | null = null;

  private setAuthInfo(value: AuthDto | null): void {
    if (!value) {
      localStorage.removeItem('authInfo');
      this._expiration = null;
    } else {
      const base64 = value.accessToken.split('.')[1];
      const token = Base64Converter.convert(base64);
      console.log(token);
      this._expiration = new Date(token.exp as number * 1000);
      localStorage.setItem('authInfo', JSON.stringify(value));
    }
    this._authentication$.next(value);
  }

  private loadAuthInfo() {
    const authInfo: AuthDto = JSON.parse(localStorage.getItem('authInfo'));
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

  private updateAuth(authInfo: AuthDto | null) {
    this.setAuthInfo(authInfo);
    if (!authInfo) {
      this.setCurrentUser(null);
      return;
    }
    this.http.get<UserDto | null>('/api/currentUser').subscribe(user => this.setCurrentUser(user))
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
      catchError(() => {
        this.router.navigate(['login']);
        return EMPTY;
      }),
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

  get currentUser$(): Observable<UserDto | null> {
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

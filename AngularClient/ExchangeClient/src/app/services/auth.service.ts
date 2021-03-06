﻿﻿import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {filter, map, tap} from 'rxjs/operators';

export enum Role {
  Administrator,
  Operator,
  Customer,
  Disabled,
}

export interface UserRegistration {
  username: string;
  password: string;
  email: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  role: string;
}

export interface AuthInfo {
  accessToken: string;
  refreshToken: string;
  userInfo: UserDto;
  serverUtcNow: string;
  accessUtcValidTo: string;
}

@Injectable()
export class AuthService {
  private static readonly MaxTimeDiffThreshold: number = Date.UTC(1, 1, 1, 1, 5) - Date.UTC(1, 1, 1, 1, 0);
  private _authInfo: AuthInfo | null = null;
  private _isAccessExpired: boolean | null = null;
  private readonly _authentication$: BehaviorSubject<AuthInfo | null>
    = new BehaviorSubject<AuthInfo | null>(null);

  private get authInfo(): AuthInfo | null {
    if (this._authInfo) {
      return this._authInfo;
    }
    this._authInfo = JSON.parse(localStorage.getItem('authInfo'));
    this.authentication$.next(this._authInfo);
    return this._authInfo;
  }

  private get checkAccessValidationExpiration(): boolean {
    if (this._isAccessExpired !== null) {
      return this._isAccessExpired;
    }
    this._isAccessExpired = localStorage.getItem('checkAccessValidationTime') === 'true';
    return this._isAccessExpired;
  }

  private setAuthInfo(value: AuthInfo | null): void {
    this._authInfo = null;
    if (!value) {
      localStorage.removeItem('authInfo');
    } else {
      localStorage.setItem('authInfo', JSON.stringify(value));
    }
    this.authentication$.next(value);
  }

  private setAccessExpired(value: boolean): void {
    this._isAccessExpired = value;
    localStorage.setItem('checkAccessValidationTime', String(value));
  }

  private getRefreshToken(): string | null {
    const authInfo = this.authInfo;
    return authInfo && authInfo.refreshToken;
  }

  private saveTokens(token: AuthInfo) {
    this.setAuthInfo(token);
    const serverUtcNow: Date = new Date(token.serverUtcNow);
    this._isAccessExpired = Math.abs(serverUtcNow.getTime() - Date.now()) < AuthService.MaxTimeDiffThreshold;
    this.setAccessExpired(this._isAccessExpired);
  }

  constructor(private http: HttpClient) {
  }

  get authentication$():BehaviorSubject<AuthInfo | null>{
    return this._authentication$;
  }

  getAccessToken(): string | null {
    const authInfo = this.authInfo;
    return authInfo && authInfo.accessToken;
  }

  isAuthPossible(): boolean {
    return !!this.getAccessToken();
  }

  getUserInfo(): UserDto | null {
    const authInfo = this.authInfo;
    return authInfo && authInfo.userInfo;
  }

  isTokenExpired(): boolean {
    const authInfo = this.authInfo;
    const checkDate = this.checkAccessValidationExpiration;
    if (!authInfo || !checkDate) {
      return false;
    }
    const currentTime = Date.now();
    const accessUtcValidTo: Date = new Date(authInfo.accessUtcValidTo);
    return !authInfo.accessToken || currentTime > accessUtcValidTo.getTime();
  }

  login(username: string, password: string): Observable<AuthInfo> {
    return this.http.post('/api/login', {
        username,
        password
      }, {observe: 'body'}
    ).pipe(
      map(token => token as AuthInfo),
      filter(token => !!token),
      tap(token => this.saveTokens(token))
    );
  }

  logout() {
    this.http.get<void>('/api/logout',{
      params: {
        refreshToken:  this.getRefreshToken()
      }
    }).subscribe();
    this.setAuthInfo(null);
    this.setAccessExpired(false);
  }

  refreshTokens(): Observable<AuthInfo> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return;
    }
    this.setAuthInfo(null);
    return this.http.get('/api/refreshToken', {params: {refreshToken}})
      .pipe(
        map(token => token as AuthInfo),
        filter(token => !!token),
        tap(token => this.saveTokens(token)),
      );
  }
}

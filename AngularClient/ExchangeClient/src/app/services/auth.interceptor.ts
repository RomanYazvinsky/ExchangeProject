﻿import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable, throwError} from 'rxjs';
import {AuthService} from './auth.service';
import {catchError, switchMap} from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.authService.isAuthPossible()) {
      return next.handle(req);
    }
    if (this.authService.isTokenExpired()) {
      return this.refreshExpiredTokenBeforeHandle(req, next);
    }
    return this.refreshExpiredTokenIfRequestFailed(req, next);
  }

  private buildAuthHeader(): string {
    return 'Bearer ' + this.authService.getAccessToken();
  }

  private setAuthHeader(req: HttpRequest<any>): HttpRequest<any> {
    return req.clone({
      headers: req.headers.set('Authorization', this.buildAuthHeader())
    });
  }

  private refreshExpiredTokenBeforeHandle(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authService.refreshTokens().pipe(
      switchMap(() => next.handle(this.setAuthHeader(req)))
    );
  }

  private refreshExpiredTokenIfRequestFailed(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const httpRequest = this.setAuthHeader(req);
    return next.handle(httpRequest).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse
          && error.status === 401
          && this.authService.isAuthPossible()
        ) {
          return this.authService.refreshTokens().pipe(
            switchMap(() => {
              return next.handle(this.setAuthHeader(req));
            })
          );
        }
        return throwError(error);
      })
    );
  }
}

import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable, Injector} from '@angular/core';
import {Observable, throwError} from 'rxjs';
import {catchError, switchMap} from 'rxjs/operators';
import {AuthService} from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private injector: Injector) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authService = this.injector.get(AuthService);
    if (!authService.isAuthPossible()) {
      return next.handle(req);
    }
    if (authService.isTokenExpired()) {
      return this.refreshExpiredTokenBeforeHandle(req, next, authService);
    }
    return this.refreshExpiredTokenIfRequestFailed(req, next, authService);
  }

  private buildAuthHeader(accessToken: string): string {
    return 'Bearer ' + accessToken;
  }

  private setAuthHeader(req: HttpRequest<any>, accessToken: string): HttpRequest<any> {
    return req.clone({
      headers: req.headers.set('Authorization', this.buildAuthHeader(accessToken))
    });
  }

  private refreshExpiredTokenBeforeHandle(req: HttpRequest<any>, next: HttpHandler, authService: AuthService): Observable<HttpEvent<any>> {
    return authService.refreshTokens().pipe(
      switchMap(auth => next.handle(this.setAuthHeader(req, auth?.accessToken)))
    );
  }

  private refreshExpiredTokenIfRequestFailed(req: HttpRequest<any>, next: HttpHandler, authService: AuthService): Observable<HttpEvent<any>> {
    const auth = authService.currentAuthentication;
    const httpRequest = this.setAuthHeader(req, auth?.accessToken);
    return next.handle(httpRequest).pipe(catchError((error) => {
        if (error instanceof HttpErrorResponse
          && error.status === 401
          && authService.isAuthPossible()
        ) {
          return authService.refreshTokens().pipe(
            switchMap(() => {
              return next.handle(this.setAuthHeader(req, auth?.accessToken));
            }),
          );
        }
        return throwError(error);
      })
    );
  }
}

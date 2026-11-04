import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, map, of, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

const MUTATING_METHODS = ['POST', 'PUT', 'DELETE', 'PATCH'];
const CSRF_EXEMPT_PATHS = [
  '/api/authorize/login',
  '/api/authorize/register',
  '/api/authorize/refresh',
  '/api/googleauth/login'
];

@Injectable()
export class AuthorizeInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  private readonly apiOrigin = 'https://localhost:44321';

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const isApiRequest = req.url.startsWith(this.apiOrigin);

    if (!isApiRequest) {
      return next.handle(req);
    }

    req = req.clone({ withCredentials: true });

    return this.withCsrfHeader(req).pipe(
      switchMap(finalReq => next.handle(finalReq)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.authService.refreshToken().pipe(
            switchMap((res) => {
              if (res) {
                return this.withCsrfHeader(req).pipe(
                  switchMap(retryReq => next.handle(retryReq))
                );
              }

              return throwError(() => error);
            })
          );
        }

        return throwError(() => error);
      })
    );
  }

  private withCsrfHeader(req: HttpRequest<any>): Observable<HttpRequest<any>> {
    if (!MUTATING_METHODS.includes(req.method) || this.isCsrfExempt(req.url)) {
      return of(req);
    }

    const csrfToken = this.authService.getCsrfToken();

    if (csrfToken) {
      return of(req.clone({ setHeaders: { 'X-CSRF-TOKEN': csrfToken } }));
    }

    return this.authService.fetchCsrfToken().pipe(
      map(res => req.clone({ setHeaders: { 'X-CSRF-TOKEN': res.csrfToken } }))
    );
  }

  private isCsrfExempt(url: string): boolean {
    const path = url.toLowerCase();
    return CSRF_EXEMPT_PATHS.some(exempt => path.includes(exempt));
  }
}

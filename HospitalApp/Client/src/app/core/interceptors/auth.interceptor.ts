import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AccountService } from '../services/account/account.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    isRefreshing = false;
    accessTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null)

    constructor(private authService: AccountService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> 
    {
        if (this.authService.getToken()) 
        {
            request = this.addToken(request, this.authService.getToken());
        }
      
        return next.handle(request).pipe(catchError(error => {
                if (error instanceof HttpErrorResponse && error.status === 401) 
                {
                    return this.handle401Error(request, next);
                } 
                else 
                {
                    return throwError(error);
                }
            })
        );
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        if (!this.isRefreshing) 
        {
            this.isRefreshing = true;
            this.accessTokenSubject.next(null);
        
            return this.authService.refreshToken().pipe(
                switchMap((token: any) => {
                this.isRefreshing = false;
                this.accessTokenSubject.next(token.jwt);

                return next.handle(this.addToken(request, token.jwt));
            }));
        } 
        else
        {
            return this.accessTokenSubject.pipe(
                filter(token => token != null),
                take(1),
                switchMap(jwt => {
                return next.handle(this.addToken(request, jwt));
            }));
        }
    }

    private addToken(request: HttpRequest<any>, token: string | null) {
        return request.clone({
            setHeaders: {
                'Authorization': `Bearer ${token}`
            }
        });
    }
}
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from 'src/app/core/models/user/user.model'
import { AuthenticatedResponse } from '../../models/user/AuthenticatedResponse';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

const AUTH_API = "https://localhost:7067";

// Credentials: to fetch user credentials from the form:
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly REFRESH_TOKEN = 'REFRESH_TOKEN';

  constructor(private http: HttpClient, private router: Router) { }

  login(form: User): Observable<AuthenticatedResponse> {
    return this.http.post<AuthenticatedResponse>(AUTH_API + "/api/Auth/login", form)
      .pipe(tap(tokens => this.saveTokens(tokens)))
  }
  
  register(form: User): Observable<string> {
    return this.http.post<string>(AUTH_API + "/api/auth/register", form, httpOptions);
  }

  refreshToken(): Observable<AuthenticatedResponse> {
    const Token = this.getToken();
    const RefreshToken = this.getRefreshToken();

    return this.http.post<AuthenticatedResponse>(AUTH_API + "/api/Auth/RefreshToken", 
      {
        Token, RefreshToken
      }).pipe(tap(tokens => this.refreshTokens(tokens))
    );
  }

  isAuthenticated(): boolean {
    return Boolean(this.getToken());
  }

  hasToken(): boolean {
    return !!localStorage.getItem(this.JWT_TOKEN);
  }

  removeTokens(): void {
    localStorage.removeItem(this.JWT_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
  }

  logout(): void {
    this.removeTokens();
    this.router.navigateByUrl('/login');
  }

  getToken(): string | null {
    return localStorage.getItem(this.JWT_TOKEN);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }

  private saveTokens(loginResponse: AuthenticatedResponse): void {
    localStorage.setItem(this.JWT_TOKEN, loginResponse.token);
    localStorage.setItem(this.REFRESH_TOKEN, loginResponse.refreshToken);
    this.router.navigateByUrl('/');
  }

  private refreshTokens(refreshTokenResponse: AuthenticatedResponse): void {
    localStorage.setItem(this.JWT_TOKEN, refreshTokenResponse.token);
    localStorage.setItem(this.REFRESH_TOKEN, refreshTokenResponse.refreshToken);
  }
}

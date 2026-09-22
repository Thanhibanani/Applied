import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  fullName: string;
  email: string;
}

const TOKEN_KEY = 'applied.accessToken';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiBase = '/api/auth';
  readonly isAuthenticated = signal(!!this.token);

  constructor(private http: HttpClient) {}

  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  register(email: string, password: string, fullName: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBase}/register`, { email, password, fullName })
      .pipe(tap((res) => this.storeToken(res)));
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBase}/login`, { email, password })
      .pipe(tap((res) => this.storeToken(res)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.isAuthenticated.set(false);
  }

  private storeToken(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.accessToken);
    this.isAuthenticated.set(true);
  }
}

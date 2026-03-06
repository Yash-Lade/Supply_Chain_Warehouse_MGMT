import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  private baseUrl = 'https://localhost:7013/api/Auth';

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  login(credentials: { username: string; password: string }) {
    return this.http.post<any>(`${this.baseUrl}/login`, credentials)
      .pipe(
        tap(response => {
          if (this.isBrowser()) {
            localStorage.setItem('token', response.token);
          }
        })
      );
  }
  register(data: any) {
    return this.http.post(`${this.baseUrl}/register`, data);
  }
  
  logout() {
    if (this.isBrowser()) {
      localStorage.removeItem('token');
    }
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getDecodedToken(): any {
    const token = this.getToken();
    if (!token) return null;

    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  }

  getRole(): number {

    const token = localStorage.getItem('token');

    if(!token) return 0;

    const payload = JSON.parse(atob(token.split('.')[1]));

    return Number(payload.role);
  }

}
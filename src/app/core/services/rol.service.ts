import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RolService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/rol';

  run(): Observable<any> {
    return this.http.post(`${this.baseUrl}/run`, {});
  }

  getAnalytics(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/analytics`);
  }

  getBelowROL(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/below`);
  }

  getDraftPOs(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/draft-pos`);
  }
}
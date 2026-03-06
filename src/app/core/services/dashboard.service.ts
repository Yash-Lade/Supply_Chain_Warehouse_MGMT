import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/dashboard'; // use your real port

  getStats(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }
}
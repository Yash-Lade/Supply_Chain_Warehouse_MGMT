import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GrnService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/grn';

  receive(poNumber: string): Observable<any> {
    return this.http.post(this.baseUrl, { poNumber });
  }

}
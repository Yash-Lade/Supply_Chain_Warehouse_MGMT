import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vendor } from '../../shared/models/vendor.model';

@Injectable({
  providedIn: 'root'
})
export class VendorService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/vendors';

  getAll(): Observable<Vendor[]> {
    return this.http.get<Vendor[]>(this.baseUrl);
  }

  create(data: any) {
    return this.http.post(this.baseUrl, data);
  }

  update(id: number, data: any) {
    return this.http.put(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
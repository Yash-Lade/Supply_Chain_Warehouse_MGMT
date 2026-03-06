import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Warehouse {
  id: number;
  name: string;
  location: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class WarehousesService {

  private http = inject(HttpClient);
  private api = 'https://localhost:7013/api/warehouses';

  getAll(): Observable<Warehouse[]> {
    return this.http.get<Warehouse[]>(this.api);
  }
}
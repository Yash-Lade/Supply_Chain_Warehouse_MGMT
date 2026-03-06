import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Item {
  id: number;
  name: string;
  sku: string;
  unitType: string;
  abcClass: string;
  xyzClass: string;
  minStockLevel: number;
  maxStockLevel: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ItemsService {

  private http = inject(HttpClient);
  private api = 'https://localhost:7013/api/items';

  getAll(): Observable<Item[]> {
    return this.http.get<Item[]>(this.api);
  }
}
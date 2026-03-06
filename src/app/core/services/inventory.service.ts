import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InventoryItem } from '../../shared/models/inventory.model';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/inventory';

  getAll(): Observable<InventoryItem[]> {
    return this.http.get<InventoryItem[]>(this.baseUrl);
  }

}
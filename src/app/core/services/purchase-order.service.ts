import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PurchaseOrder } from '../../shared/models/purchase-order.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {

  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7013/api/PurchaseOrders';

  getAll(): Observable<PurchaseOrder[]> {
    return this.http.get<PurchaseOrder[]>(this.baseUrl);
  }

  getById(id: number): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(`${this.baseUrl}/${id}`);
  }

  create(data: any) {
    return this.http.post(this.baseUrl, data);
  }

  submit(id: number) {
    return this.http.post(`${this.baseUrl}/${id}/submit`, {});
  }

  approve(poId:number, body:any){
      return this.http.put(`${this.baseUrl}/${poId}/approve`, body);
  }

  getByPoNumber(poId: string) {
    return this.http.get<any>(`${this.baseUrl}?poNumber=${poId}`);
  }
}

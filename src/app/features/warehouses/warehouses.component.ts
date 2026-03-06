import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WarehousesService, Warehouse } from '../../core/services/warehouses.service';

@Component({
  standalone: true,
  selector: 'app-warehouses',
  imports: [CommonModule],
  templateUrl: './warehouses.component.html'
})
export class WarehousesComponent implements OnInit {

  private warehouseService = inject(WarehousesService);

  warehouses: Warehouse[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadWarehouses();
  }

  loadWarehouses() {
    this.loading = true;

    this.warehouseService.getAll().subscribe({
      next: data => {
        this.warehouses = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}
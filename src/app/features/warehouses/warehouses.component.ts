import { Component, OnInit, inject, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WarehousesService, Warehouse } from '../../core/services/warehouses.service';

import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';

@Component({
  standalone: true,
  selector: 'app-warehouses',
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule
  ],
  templateUrl: './warehouses.component.html'
})
export class WarehousesComponent implements OnInit, AfterViewInit {

  private warehousesService = inject(WarehousesService);

  displayedColumns: string[] = [
    'name',
    'location',
    'isActive'
  ];

  dataSource = new MatTableDataSource<Warehouse>([]);

  loading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.loadWarehouses();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadWarehouses() {

    this.loading = true;

    this.warehousesService.getAll().subscribe({
      next: data => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });

  }

}
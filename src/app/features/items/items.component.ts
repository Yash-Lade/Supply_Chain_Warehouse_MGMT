import { Component, OnInit, inject, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemsService, Item } from '../../core/services/items.service';

import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';

@Component({
  standalone: true,
  selector: 'app-items',
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule
  ],
  templateUrl: './items.component.html'
})
export class ItemsComponent implements OnInit, AfterViewInit {

  private itemsService = inject(ItemsService);

  loading = false;

  displayedColumns: string[] = [
    'name',
    'sku',
    'unitType',
    'abcClass',
    'xyzClass',
    'minStockLevel',
    'maxStockLevel'
  ];

  dataSource = new MatTableDataSource<Item>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.loadItems();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadItems() {

    this.loading = true;

    this.itemsService.getAll().subscribe({
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
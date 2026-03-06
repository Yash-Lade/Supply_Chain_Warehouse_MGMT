import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-low-stock-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule
  ],
  templateUrl: './low-stock-table.component.html',
  styleUrls: ['./low-stock-table.component.scss']
})
export class LowStockTableComponent implements OnInit {

  displayedColumns: string[] = [
    'itemName',
    'warehouse',
    'stock',
    'rol',
    'status'
  ];

  dataSource: any[] = [];

  ngOnInit() {
    this.loadLowStock();
  }

  loadLowStock() {

    // TEMP MOCK DATA (until API connected)

    this.dataSource = [
      {
        itemName: 'Paracetamol',
        warehouse: 'Main Store',
        stock: 15,
        rol: 20
      },
      {
        itemName: 'Amoxicillin',
        warehouse: 'Pharmacy',
        stock: 8,
        rol: 25
      },
      {
        itemName: 'Insulin',
        warehouse: 'Cold Storage',
        stock: 5,
        rol: 15
      }
    ];

  }

  getStatus(item: any) {

    if (item.stock <= item.rol * 0.5)
      return 'Critical';

    if (item.stock <= item.rol)
      return 'Low';

    return 'Healthy';

  }

}
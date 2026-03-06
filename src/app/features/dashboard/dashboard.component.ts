// import { Component, OnInit, inject } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { DashboardService } from '../../core/services/dashboard.service';
// import { MatCardModule } from '@angular/material/card';
// import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
// import { BaseChartDirective } from 'ng2-charts';
// import { ChartConfiguration } from 'chart.js';
// import { forkJoin } from 'rxjs';
// @Component({
//   standalone: true,
//   selector: 'app-dashboard',
//   imports: [
//     CommonModule,
//     MatCardModule,
//     MatProgressSpinnerModule,
//     BaseChartDirective
//   ],
//   templateUrl: './dashboard.component.html',
//   styleUrls: ['./dashboard.component.scss']
// })
// export class DashboardComponent implements OnInit {

//   private dashboardService = inject(DashboardService);

//   loading: boolean = true;
//   kpis: any = {};

//   chartData: ChartConfiguration<'line'>['data'] = {
//     labels: [],
//     datasets: [
//       {
//         data: [],
//         label: 'PO Trend'
//       }
//     ]
//   };

//   chartOptions: ChartConfiguration<'line'>['options'] = {
//     responsive: true
//   };

//   ngOnInit(): void {
//     console.log('Dashboard Init Running');
//     this.loadDashboard();
//   }
  
//   loadDashboard() {
//   this.loading = true;

//   forkJoin({
//     stats: this.dashboardService.getStats()
//   }).subscribe({
//     next: (result) => {

//       this.kpis = result.stats;

//       this.chartData.labels = ['Total', 'Pending', 'Approved', 'Received'];
//       this.chartData.datasets[0].data = [
//         result.stats.totalPOs,
//         result.stats.pendingApprovals,
//         result.stats.approvedPOs,
//         result.stats.receivedPOs
//       ];

//       this.loading = false;
//     },
//     error: (err) => {
//       console.error(err);
//       this.loading = false;
//     }
//   });
// }

// }

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { BaseChartDirective } from 'ng2-charts';
import { PendingApprovalsComponent } from "./pending-approvals/pending-approvals.component";
import { LowStockTableComponent } from './low-stock-table/low-stock-table';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    BaseChartDirective,
    PendingApprovalsComponent,
    LowStockTableComponent
],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  totalPOs = 12;
  pendingApprovals = 5;
  lowStock = 5;
  receivedToday = 2;

  poChartData = {
    labels: ['Jan','Feb','Mar','Apr','May','Jun'],
    datasets: [
      {
        data: [12,19,3,5,2,3],
        label: 'Purchase Orders'
      }
    ]
  };

  stockChartData = {
    labels: ['Paracetamol','Amoxicillin','Insulin'],
    datasets: [
      {
        data: [30,40,30]
      }
    ]
  };

}
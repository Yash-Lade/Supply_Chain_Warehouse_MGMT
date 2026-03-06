import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RolService } from '../../core/services/rol.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

@Component({
  standalone: true,
  selector: 'app-rol',
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    BaseChartDirective
  ],
  templateUrl: './rol.component.html',
  styleUrls: ['./rol.component.scss']
})
export class RolComponent implements OnInit {

  private rolService = inject(RolService);
  private snackBar = inject(MatSnackBar);

  loading = false;
  belowRolItems: any[] = [];
  draftPOs: any[] = [];

  chartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      { data: [], label: 'Current Stock' },
      { data: [], label: 'ROL' }
    ]
  };

  chartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    plugins: {
      legend: { display: true }
    }
  };

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;

    this.rolService.getAnalytics().subscribe(data => {
      this.chartData.labels = data.map(x => x.itemName);
      this.chartData.datasets[0].data = data.map(x => x.currentStock);
      this.chartData.datasets[1].data = data.map(x => x.rol);
    });

    this.rolService.getBelowROL().subscribe(data => {
      this.belowRolItems = data;
    });

    this.rolService.getDraftPOs().subscribe(data => {
      this.draftPOs = data;
      this.loading = false;
    });
  }

  runRol() {
    this.loading = true;

    this.rolService.run().subscribe({
      next: () => {
        this.snackBar.open('ROL recalculation triggered', 'Close', { duration: 3000 });
        this.loadData();
      },
      error: () => {
        this.snackBar.open('ROL execution failed', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

}
import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { PurchaseOrderService } from '../../core/services/purchase-order.service';
import { PurchaseOrder } from '../../shared/models/purchase-order.model';
import { AuthService } from '../../core/services/auth.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CreatePoDialogComponent } from './create-po-dialog/create-po-dialog.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  standalone: true,
  selector: 'app-purchase-orders',
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './purchase-orders.component.html',
  styleUrls: ['./purchase-orders.component.scss']
})
export class PurchaseOrdersComponent implements OnInit {

  private poService = inject(PurchaseOrderService);
  private snackBar = inject(MatSnackBar);
  private authService = inject(AuthService);
  private dialog = inject(MatDialog);
  loading = false;

  displayedColumns = ['poNumber', 'vendorName', 'totalAmount', 'status', 'actions'];
  dataSource = new MatTableDataSource<PurchaseOrder>();

  role = this.authService.getRole();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.loadPOs();
  }

  loadPOs() {
    this.loading = true;

    this.poService.getAll().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  submit(po: PurchaseOrder) {
    this.poService.submit(po.id).subscribe({
      next: () => {
        this.snackBar.open('Submitted for approval', 'Close', { duration: 3000 });
        this.loadPOs();
      }
    });
  }

  approve(po: PurchaseOrder, action: 'Approved' | 'Rejected') {
    const confirm = window.confirm(`Are you sure you want to ${action} this PO?`);
    if (!confirm) return;

    this.poService.approve(po.id, { action }).subscribe({
      next: () => {
        this.snackBar.open(`PO ${action}`, 'Close', { duration: 3000 });
        this.loadPOs();
      }
    });
  }

  canApprove(po: PurchaseOrder): boolean {
    if (po.status !== 'PendingApproval') return false;
    // HOD - L1
    if (this.role === 2 && !po.level1Approved) {
      return true;
    }
    // Finance - L2
    if (this.role === 3 && po.level1Approved && !po.level2Approved) {
      return true;
    }
    // Director - L3
    if (this.role === 4 && po.level1Approved && po.level2Approved && !po.level3Approved) {
      return true;
    }
    return false;
  }

  openCreateDialog() {
    const dialogRef = this.dialog.open(CreatePoDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.poService.create(result).subscribe({
        next: () => {
          this.snackBar.open('PO Created', 'Close', { duration: 3000 });
          this.loadPOs();
        }
      });
    });
  }
}
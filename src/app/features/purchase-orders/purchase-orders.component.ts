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
import { ChangeDetectorRef } from '@angular/core';
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

  private cdr = inject(ChangeDetectorRef);

  purchaseOrders:PurchaseOrder[] = [];
  displayedColumns = [
    'poNumber',
    'vendorName',
    'totalAmount',
    'status',
    'action'
  ];
  dataSource = new MatTableDataSource<PurchaseOrder>();

  role = this.authService.getRole();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.loadPOs();
    console.log("ROLE:", this.role);
  }

  loadPOs() {

    this.loading = true;
    this.poService.getAll().subscribe({

      next: (data) => {

        const processed = data.map(po => ({
          ...po,
          canApprove: this.canApprove(po)
        }));

        this.purchaseOrders = processed;
        this.dataSource.data = processed;

        this.loading = false;

        this.cdr.detectChanges();
      },

      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });

  }
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
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

  canApprove(po:PurchaseOrder):boolean{

    if (!po.approvals || po.approvals.length === 0)
    return false;
    const level = this.role - 1;

    const approval = po.approvals.find(a=>a.level === level);

    if(!approval) return false;

    if(approval.status !== 'Pending') return false;

    const previousApproved =
      po.approvals
        .filter(a=>a.level < level)
        .every(a=>a.status === 'Approved');

    return previousApproved;
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

approvePO(poId:number){
  this.poService.approve(poId,{
  action:'Approved'
  }).subscribe(()=>{

  this.loadPOs();

  });
  }

  getStatusText(po:PurchaseOrder):string{

  if (!po) return 'Unknown';

  // Draft stage
  if (po.status === 'Draft')
    return 'Draft';

  // Received stage
  if (po.status === 'Received')
    return 'Received';

  // Rejected stage
  if (po.status === 'Rejected')
    return 'Rejected';

  // Approval workflow
  if (po.status === 'PendingApproval' && po.approvals?.length) {

    const approvals = po.approvals;

    if (approvals[0]?.status === 'Pending')
      return 'Pending HOD';

    if (
      approvals[0]?.status === 'Approved' &&
      approvals[1]?.status === 'Pending'
    )
      return 'Pending Finance';

    if (
      approvals[1]?.status === 'Approved' &&
      approvals[2]?.status === 'Pending'
    )
      return 'Pending Director';

    if (approvals[2]?.status === 'Approved')
      return 'Approved';
  }

  return 'Unknown';
}

getStatusClass(po: PurchaseOrder): string {

  if (!po.approvals || po.approvals.length === 0)
    return 'status-pending';

  const approvals = po.approvals;

  if (approvals[0]?.status === 'Pending')
    return 'status-pending';

  if (
    approvals[0]?.status === 'Approved' &&
    approvals[1]?.status === 'Pending'
  )
    return 'status-warning';

  if (
    approvals[1]?.status === 'Approved' &&
    approvals[2]?.status === 'Pending'
  )
    return 'status-warning';

  if (approvals[2]?.status === 'Approved')
    return 'status-approved';

  return 'status-pending';
}
}
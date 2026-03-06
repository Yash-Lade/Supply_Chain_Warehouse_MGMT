import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { GrnService } from '../../core/services/grn.service';
import { PurchaseOrderService } from '../../core/services/purchase-order.service';

@Component({
  standalone: true,
  selector: 'app-grn',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './grn.component.html',
  styleUrls: ['./grn.component.scss']
})
export class GrnComponent {

  private fb = inject(FormBuilder);
  private grnService = inject(GrnService);
  private poService = inject(PurchaseOrderService);
  private snackBar = inject(MatSnackBar);

  loading = false;
  poDetails: any = null;

  form = this.fb.group({
    poNumber: ['', Validators.required]
  });

  searchPO() {

    if (this.form.invalid) return;

    this.loading = true;

    const poId = this.form.value.poNumber!;

    this.poService.getByPoNumber(poId).subscribe({
      next: (data) => {
        this.poDetails = data;
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('PO not found', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  confirmGRN() {

    if (!this.poDetails) return;

    const confirm = window.confirm('Confirm receipt of goods?');
    if (!confirm) return;

    this.loading = true;

    this.grnService.receive(this.poDetails.poNumber).subscribe({
      next: () => {
        this.snackBar.open('GRN received successfully', 'Close', { duration: 3000 });
        this.poDetails.status = 'Received';
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Error processing GRN', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

}
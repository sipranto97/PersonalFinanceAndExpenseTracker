import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { ExpenseShareService } from '../../services/expense-share.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-share-expense-modal',
    templateUrl: './share-expense-modal.component.html',
    styleUrl: './share-expense-modal.component.css',
    standalone: false
})
export class ShareExpenseModalComponent {
  shareForm: FormGroup;
  errorMessage: string | null = null;
  shareUrl: string | null = null;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareExpenseModalComponent>,
    private expenseShareService: ExpenseShareService,
    private toastr: ToastrService
  ) {
    const today = new Date().toISOString().split('T')[0];

    this.shareForm = this.fb.group({
      startDate: [today],
      endDate: [today],
      setExpiration: [false],
      expiresAt: [today]
    });
  }

  generateLink(): void {
    this.errorMessage = null;

    const formValue = this.shareForm.getRawValue();

    if (!formValue.startDate || !formValue.endDate) {
      this.errorMessage = 'Please select both a start and end date.';
      return;
    }

    if (formValue.startDate > formValue.endDate) {
      this.errorMessage = 'Start date must be before the end date.';
      return;
    }

    if (formValue.setExpiration && !formValue.expiresAt) {
      this.errorMessage = 'Please select an expiration date.';
      return;
    }

    const payload = {
      startDate: formValue.startDate,
      endDate: formValue.endDate,
      expiresAt: formValue.setExpiration ? formValue.expiresAt : null
    };

    this.isSubmitting = true;

    this.expenseShareService.createShareLink(payload).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        this.shareUrl = `${window.location.origin}/share/expense/${res.token}`;
      },
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = 'Something went wrong while generating the link.';
      }
    });
  }

  copyLink(): void {
    if (!this.shareUrl) {
      return;
    }

    navigator.clipboard.writeText(this.shareUrl).then(() => {
      this.toastr.success('Link copied to clipboard.');
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}

import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ChartShareService } from '../../services/chart-share.service';
import { ChartType } from '../../models/dashboard-chart.model';
import { ToastrService } from 'ngx-toastr';

export interface ShareChartModalData {
  chartType: ChartType;
  startDate: string;
  endDate: string;
}

@Component({
    selector: 'app-share-chart-modal',
    templateUrl: './share-chart-modal.component.html',
    styleUrl: './share-chart-modal.component.css',
    standalone: false
})
export class ShareChartModalComponent {
  shareForm: FormGroup;
  errorMessage: string | null = null;
  shareUrl: string | null = null;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareChartModalComponent>,
    private chartShareService: ChartShareService,
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: ShareChartModalData
  ) {
    this.shareForm = this.fb.group({
      startDate: [data.startDate],
      endDate: [data.endDate],
      setExpiration: [false],
      expiresAt: [data.endDate]
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
      chartType: this.data.chartType,
      startDate: formValue.startDate,
      endDate: formValue.endDate,
      expiresAt: formValue.setExpiration ? formValue.expiresAt : null
    };

    this.isSubmitting = true;

    this.chartShareService.createShareLink(payload).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        this.shareUrl = `${window.location.origin}/share/chart/${res.token}`;
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

import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

export interface ExportIncomeFilter {
  exportAll: boolean;
  startDate: string;
  endDate: string;
}

@Component({
    selector: 'app-export-income-modal',
    templateUrl: './export-income-modal.component.html',
    styleUrl: './export-income-modal.component.css',
    standalone: false
})
export class ExportIncomeModalComponent {
  exportForm: FormGroup;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ExportIncomeModalComponent>
  ) {
    const today = new Date().toISOString().split('T')[0];

    this.exportForm = this.fb.group({
      exportAll: [true],
      startDate: [today],
      endDate: [today]
    });
  }

  export(): void {
    this.errorMessage = null;

    const formValue = this.exportForm.getRawValue();

    if (!formValue.exportAll) {
      if (!formValue.startDate || !formValue.endDate) {
        this.errorMessage = 'Please select both a start and end date.';
        return;
      }

      if (new Date(formValue.startDate) > new Date(formValue.endDate)) {
        this.errorMessage = 'Start date must be before the end date.';
        return;
      }
    }

    const result: ExportIncomeFilter = {
      exportAll: formValue.exportAll,
      startDate: formValue.startDate,
      endDate: formValue.endDate
    };

    this.dialogRef.close(result);
  }

  close(): void {
    this.dialogRef.close();
  }
}

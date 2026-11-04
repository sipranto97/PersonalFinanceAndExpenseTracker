import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'app-add-savings-goal-modal',
    templateUrl: './add-savings-goal-modal.component.html',
    styleUrl: './add-savings-goal-modal.component.css',
    standalone: false
})
export class AddSavingsGoalModalComponent implements OnInit {
  goalForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddSavingsGoalModalComponent>
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {

    this.goalForm = this.fb.group({
      name: [null, Validators.required],
      targetAmount: [
        null,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      targetDate: [
        this.toDateOnly(new Date()),
        Validators.required
      ],
      monthlyContribution: [null]
    });

  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  save(): void {

    if (this.goalForm.invalid) {
      this.goalForm.markAllAsTouched();
      return;
    }

    const formValue = this.goalForm.getRawValue();

    const goal = {
      name: formValue.name,
      targetAmount: Number(formValue.targetAmount),
      targetDate: new Date(formValue.targetDate),
      monthlyContribution: formValue.monthlyContribution ? Number(formValue.monthlyContribution) : null
    };

    this.dialogRef.close(goal);
  }

  close(): void {
    this.dialogRef.close();
  }
}

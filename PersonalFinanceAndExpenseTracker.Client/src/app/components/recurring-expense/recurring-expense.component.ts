import { Component, OnInit } from '@angular/core';
import { RecurringExpense } from '../../models/recurring-expense.model';
import { MatDialog } from '@angular/material/dialog';
import { AddRecurringExpenseModalComponent } from '../add-recurring-expense-modal/add-recurring-expense-modal.component';
import { RecurringExpenseService } from '../../services/recurring-expense.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-recurring-expense',
    templateUrl: './recurring-expense.component.html',
    styleUrl: './recurring-expense.component.css',
    standalone: false
})
export class RecurringExpenseComponent implements OnInit {
  recurringExpenses: RecurringExpense[] = [];

  constructor(
    private dialog: MatDialog,
    private recurringExpenseService: RecurringExpenseService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getRecurringExpenses();
  }

  getRecurringExpenses(): void {
    this.recurringExpenseService.getAllRecurringExpenses().subscribe((recurringExpenses: any) => {
      this.recurringExpenses = recurringExpenses;
    })
  }

  openAddModal(): void {

    const dialogRef = this.dialog.open(
      AddRecurringExpenseModalComponent,
      {
        width: '550px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.recurringExpenseService.createRecurringExpense(result).subscribe(() => {
        this.toastr.success("Recurring expense created successfully.");
        this.getRecurringExpenses();
      })

    });
  }

  openEditModal(recurringExpense: RecurringExpense): void {

    const dialogRef = this.dialog.open(
      AddRecurringExpenseModalComponent,
      {
        width: '550px',
        disableClose: true,
        data: recurringExpense
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.recurringExpenseService.updateRecurringExpense(recurringExpense.id, result).subscribe(() => {
        this.toastr.success("Recurring expense updated successfully.");
        this.getRecurringExpenses();
      })

    });
  }

  cancelRecurringExpense(id: number): void {
    this.recurringExpenseService.cancelRecurringExpense(id).subscribe(() => {
      this.toastr.success("Recurring expense cancelled successfully.");
      this.getRecurringExpenses();
    })
  }
}

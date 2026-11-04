import { Component, OnInit } from '@angular/core';
import { RecurringIncome } from '../../models/recurring-income.model';
import { MatDialog } from '@angular/material/dialog';
import { AddRecurringIncomeModalComponent } from '../add-recurring-income-modal/add-recurring-income-modal.component';
import { RecurringIncomeService } from '../../services/recurring-income.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-recurring-income',
    templateUrl: './recurring-income.component.html',
    styleUrl: './recurring-income.component.css',
    standalone: false
})
export class RecurringIncomeComponent implements OnInit {
  recurringIncomes: RecurringIncome[] = [];

  constructor(
    private dialog: MatDialog,
    private recurringIncomeService: RecurringIncomeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getRecurringIncomes();
  }

  getRecurringIncomes(): void {
    this.recurringIncomeService.getAllRecurringIncomes().subscribe((recurringIncomes: any) => {
      this.recurringIncomes = recurringIncomes;
    })
  }

  openAddModal(): void {

    const dialogRef = this.dialog.open(
      AddRecurringIncomeModalComponent,
      {
        width: '550px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.recurringIncomeService.createRecurringIncome(result).subscribe(() => {
        this.toastr.success("Recurring income created successfully.");
        this.getRecurringIncomes();
      })

    });
  }

  openEditModal(recurringIncome: RecurringIncome): void {

    const dialogRef = this.dialog.open(
      AddRecurringIncomeModalComponent,
      {
        width: '550px',
        disableClose: true,
        data: recurringIncome
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.recurringIncomeService.updateRecurringIncome(recurringIncome.id, result).subscribe(() => {
        this.toastr.success("Recurring income updated successfully.");
        this.getRecurringIncomes();
      })

    });
  }

  cancelRecurringIncome(id: number): void {
    this.recurringIncomeService.cancelRecurringIncome(id).subscribe(() => {
      this.toastr.success("Recurring income cancelled successfully.");
      this.getRecurringIncomes();
    })
  }
}

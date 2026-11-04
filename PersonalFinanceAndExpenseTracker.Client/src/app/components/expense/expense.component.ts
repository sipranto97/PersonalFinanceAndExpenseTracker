import { Component, OnInit } from '@angular/core';
import { Category } from '../../models/income.model';
import { Expense } from '../../models/expense.model';
import { MatDialog } from '@angular/material/dialog';
import { AddExpenseModalComponent } from '../add-expense-modal/add-expense-modal.component';
import { ExportExpenseModalComponent, ExportExpenseFilter } from '../export-expense-modal/export-expense-modal.component';
import { ShareExpenseModalComponent } from '../share-expense-modal/share-expense-modal.component';
import { ExpenseService } from '../../services/expense.service';
import { ToastrService } from 'ngx-toastr';
import { exportRowsToExcel } from '../../utils/excel-export.util';

@Component({
    selector: 'app-expense',
    templateUrl: './expense.component.html',
    styleUrl: './expense.component.css',
    standalone: false
})
export class ExpenseComponent implements OnInit{
  expenses: Expense[] = [];
  categories: Category[] = [];

  editingId: number | null = null;
  editCategoryId: number | null = null;
  editAmount: number | null = null;
  editDate: string = '';

  sortColumn: 'category' | 'amount' | 'trxDate' = 'trxDate';
  sortDirection: 'asc' | 'desc' = 'desc';

  page = 1;
  pageSize = 10;

  constructor(
    private dialog: MatDialog,
    private expenseService: ExpenseService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getExpenses();
    this.getCategories();
  }

  getExpenses() {
    this.expenseService.getAllExpense().subscribe((expenses: any) => {
      this.expenses = expenses;
      this.page = Math.min(this.page, Math.max(1, Math.ceil(expenses.length / this.pageSize)));
    })
  }

  getCategories() {
    this.expenseService.getCategories().subscribe((categories: any) => {
      this.categories = categories;
    })
  }

  get pagedExpenses(): Expense[] {
    const start = (this.page - 1) * this.pageSize;
    return this.sortedExpenses.slice(start, start + this.pageSize);
  }

  get sortedExpenses(): Expense[] {
    const column = this.sortColumn;
    const direction = this.sortDirection === 'asc' ? 1 : -1;

    return [...this.expenses].sort((a, b) => {
      let valueA: any = a[column];
      let valueB: any = b[column];

      if (column === 'trxDate') {
        valueA = new Date(valueA).getTime();
        valueB = new Date(valueB).getTime();
      } else if (column === 'category') {
        valueA = valueA?.toLowerCase();
        valueB = valueB?.toLowerCase();
      }

      if (valueA < valueB) return -1 * direction;
      if (valueA > valueB) return 1 * direction;

      const dateDiff = new Date(b.trxDate).getTime() - new Date(a.trxDate).getTime();
      return dateDiff !== 0 ? dateDiff : b.id - a.id;
    });
  }

  sortBy(column: 'category' | 'amount' | 'trxDate'): void {
    if (this.sortColumn !== column) {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    } else {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    }

    this.page = 1;
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.page = 1;
  }

  openAddExpenseModal(): void {

    const dialogRef = this.dialog.open(
      AddExpenseModalComponent,
      {
        width: '550px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.expenseService.createExpense(result).subscribe(res => {
        this.toastr.success("Expense added successfully.");
        this.getExpenses();
      })

    });
  }

  openExportModal(): void {

    const dialogRef = this.dialog.open(
      ExportExpenseModalComponent,
      {
        width: '450px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe((result: ExportExpenseFilter) => {

      if (!result) {
        return;
      }

      let dataToExport = this.expenses;

      if (!result.exportAll) {
        dataToExport = this.expenses.filter(expense => {
          const trxDate = this.toDateOnly(expense.trxDate);
          return trxDate >= result.startDate && trxDate <= result.endDate;
        });
      }

      if (!dataToExport.length) {
        this.toastr.error("No expense records found for the selected range.");
        return;
      }

      this.exportToExcel(dataToExport);
    });
  }

  openShareModal(): void {
    this.dialog.open(
      ShareExpenseModalComponent,
      {
        width: '480px',
        disableClose: true
      }
    );
  }

  exportToExcel(data: Expense[]): void {
    const rows = data.map(expense => ({
      Category: expense.category,
      Amount: expense.amount,
      Date: this.toDdMmYyyy(expense.trxDate)
    }));

    const fileName = `expense_export_${this.toDateOnly(new Date())}.xlsx`;
    exportRowsToExcel(rows, 'Expense', fileName);
  }

  private toDateOnly(date: Date | string): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private toDdMmYyyy(date: Date | string): string {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
  }

  deleteExpense(id: number): void {
    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        this.toastr.success("Expense deleted successfully.");
        this.getExpenses();
      },
      error: (err) => {
        this.toastr.error(typeof err.error === 'string' ? err.error : "Unable to delete this expense.");
      }
    })
  }

  startEdit(expense: Expense): void {
    this.editingId = expense.id;
    this.editCategoryId = expense.categoryId;
    this.editAmount = expense.amount;
    this.editDate = this.toDateOnly(expense.trxDate);
  }

  cancelEdit(): void {
    this.editingId = null;
  }

  saveEdit(expense: Expense): void {
    if (!this.editCategoryId || this.editAmount == null || this.editAmount < 1 || !this.editDate) {
      this.toastr.error("Please provide a valid category, amount and date.");
      return;
    }

    const category = this.categories.find(c => c.id === this.editCategoryId);

    const payload = {
      categoryId: this.editCategoryId,
      category: category?.name,
      amount: Number(this.editAmount),
      trxDate: new Date(this.editDate)
    };

    this.expenseService.updateExpense(expense.id, payload).subscribe(() => {
      this.toastr.success("Expense updated successfully.");
      this.editingId = null;
      this.getExpenses();
    })
  }
}

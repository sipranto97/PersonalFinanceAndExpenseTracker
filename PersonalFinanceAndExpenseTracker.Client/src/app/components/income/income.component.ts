import { Component, OnInit } from '@angular/core';
import { Category, Income } from '../../models/income.model';
import { MatDialog } from '@angular/material/dialog';
import { AddIncomeModalComponent } from '../add-income-modal/add-income-modal.component';
import { ExportIncomeModalComponent, ExportIncomeFilter } from '../export-income-modal/export-income-modal.component';
import { ShareIncomeModalComponent } from '../share-income-modal/share-income-modal.component';
import { IncomeService } from '../../services/income.service';
import { ToastrService } from 'ngx-toastr';
import { exportRowsToExcel } from '../../utils/excel-export.util';

@Component({
    selector: 'app-income',
    templateUrl: './income.component.html',
    styleUrl: './income.component.css',
    standalone: false
})
export class IncomeComponent implements OnInit{
  incomes: Income[] = [];
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
    private incomeService: IncomeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getIncomes();
    this.getCategories();
  }

  getIncomes() {
    this.incomeService.getAllIncome().subscribe((incomes: any) => {
      this.incomes = incomes;
      this.page = Math.min(this.page, Math.max(1, Math.ceil(incomes.length / this.pageSize)));
    })
  }

  getCategories() {
    this.incomeService.getCategories().subscribe((categories: any) => {
      this.categories = categories;
    })
  }

  get pagedIncomes(): Income[] {
    const start = (this.page - 1) * this.pageSize;
    return this.sortedIncomes.slice(start, start + this.pageSize);
  }

  get sortedIncomes(): Income[] {
    const column = this.sortColumn;
    const direction = this.sortDirection === 'asc' ? 1 : -1;

    return [...this.incomes].sort((a, b) => {
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

  openAddIncomeModal(): void {

    const dialogRef = this.dialog.open(
      AddIncomeModalComponent,
      {
        width: '550px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      this.incomeService.createIncome(result).subscribe(res => {
        this.toastr.success("Income added successfully.");
        this.getIncomes();
      })

    });
  }

  openExportModal(): void {

    const dialogRef = this.dialog.open(
      ExportIncomeModalComponent,
      {
        width: '450px',
        disableClose: true
      }
    );

    dialogRef.afterClosed().subscribe((result: ExportIncomeFilter) => {

      if (!result) {
        return;
      }

      let dataToExport = this.incomes;

      if (!result.exportAll) {
        dataToExport = this.incomes.filter(income => {
          const trxDate = this.toDateOnly(income.trxDate);
          return trxDate >= result.startDate && trxDate <= result.endDate;
        });
      }

      if (!dataToExport.length) {
        this.toastr.error("No income records found for the selected range.");
        return;
      }

      this.exportToExcel(dataToExport);
    });
  }

  openShareModal(): void {
    this.dialog.open(
      ShareIncomeModalComponent,
      {
        width: '480px',
        disableClose: true
      }
    );
  }

  exportToExcel(data: Income[]): void {
    const rows = data.map(income => ({
      Category: income.category,
      Amount: income.amount,
      Date: this.toDdMmYyyy(income.trxDate)
    }));

    const fileName = `income_export_${this.toDateOnly(new Date())}.xlsx`;
    exportRowsToExcel(rows, 'Income', fileName);
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

  deleteIncome(id: number): void {
    this.incomeService.deleteIncome(id).subscribe({
      next: () => {
        this.toastr.success("Income deleted successfully.");
        this.getIncomes();
      },
      error: (err) => {
        this.toastr.error(typeof err.error === 'string' ? err.error : "Unable to delete this income.");
      }
    })
  }

  startEdit(income: Income): void {
    this.editingId = income.id;
    this.editCategoryId = income.categoryId;
    this.editAmount = income.amount;
    this.editDate = this.toDateOnly(income.trxDate);
  }

  cancelEdit(): void {
    this.editingId = null;
  }

  saveEdit(income: Income): void {
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

    this.incomeService.updateIncome(income.id, payload).subscribe(() => {
      this.toastr.success("Income updated successfully.");
      this.editingId = null;
      this.getIncomes();
    })
  }
}

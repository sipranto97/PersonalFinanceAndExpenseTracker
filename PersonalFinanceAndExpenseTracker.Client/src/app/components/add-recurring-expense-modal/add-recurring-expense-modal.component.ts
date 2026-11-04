import { Component, ElementRef, HostListener, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Category } from '../../models/income.model';
import { RecurringExpense } from '../../models/recurring-expense.model';
import { ExpenseService } from '../../services/expense.service';

@Component({
    selector: 'app-add-recurring-expense-modal',
    templateUrl: './add-recurring-expense-modal.component.html',
    styleUrl: './add-recurring-expense-modal.component.css',
    standalone: false
})
export class AddRecurringExpenseModalComponent implements OnInit {
  recurringExpenseForm!: FormGroup;

  searchText = '';

  categories: Category[] = []

  filteredCategories = [...this.categories];
  showDropdown = false;

  daysOfMonth: number[] = Array.from({ length: 31 }, (_, i) => i + 1);

  isEditMode = false;

  @ViewChild('dropdownContainer') dropdownContainer!: ElementRef;

  @HostListener('document:click', ['$event'])
  onOutsideClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    if (this.dropdownContainer?.nativeElement.contains(target)) {
      return;
    }

    this.showDropdown = false;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddRecurringExpenseModalComponent>,
    private expenseService: ExpenseService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: RecurringExpense | null
  ) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;

    this.expenseService.getCategories().subscribe((categories: any) => {
      this.categories = categories;
      this.filteredCategories = categories;
    })

    this.initializeForm();
  }

  initializeForm(): void {

    this.recurringExpenseForm = this.fb.group({
      category: [
        this.data ? { id: this.data.categoryId, name: this.data.category } : null,
        Validators.required
      ],
      amount: [
        this.data?.amount ?? null,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      dayOfMonth: [
        this.data?.dayOfMonth ?? null,
        Validators.required
      ]
    });

    if (this.data) {
      this.searchText = this.data.category;
    }
  }

  onSearch(event: any): void {
    this.searchText = event.term;
  }

  openDropdown(): void {
    this.showDropdown = true;
  }

  filterCategories(): void {
    const search = this.searchText?.toLowerCase().trim() || '';

    this.filteredCategories = this.categories.filter(c =>
      c.name.toLowerCase().includes(search)
    );

    this.showDropdown = true;
  }

  selectCategory(category: Category): void {
    this.recurringExpenseForm.patchValue({
      category
    });

    this.searchText = category.name;
    this.showDropdown = false;
  }

  createCategory(name: string): void {
    if (!name?.trim()) return;

    const exists = this.categories.some(
      x => x.name.toLowerCase() === name.toLowerCase()
    );

    if (exists) return;

    const newCategory: Category = {
      id: -1,
      name: name.trim()
    };

    this.categories.push(newCategory);
    this.filteredCategories = [...this.categories];

    this.selectCategory(newCategory);
  }

  save(): void {

    if (this.recurringExpenseForm.invalid) {
      this.recurringExpenseForm.markAllAsTouched();
      return;
    }

    const formValue = this.recurringExpenseForm.getRawValue();

    const recurringExpense = {
      categoryId: formValue.category.id,
      category: formValue.category.name,
      amount: Number(formValue.amount),
      dayOfMonth: Number(formValue.dayOfMonth)
    };

    this.dialogRef.close(recurringExpense);
  }

  close(): void {
    this.dialogRef.close();
  }
}

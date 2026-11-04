import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { Category } from '../../models/income.model';
import { IncomeService } from '../../services/income.service';

@Component({
    selector: 'app-add-income-modal',
    templateUrl: './add-income-modal.component.html',
    styleUrl: './add-income-modal.component.css',
    standalone: false
})
export class AddIncomeModalComponent implements OnInit {
  incomeForm!: FormGroup;

  searchText = '';

  categories: Category[] = []

  filteredCategories = [...this.categories];
  showDropdown = false;
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
    private dialogRef: MatDialogRef<AddIncomeModalComponent>,
    private incomeService: IncomeService
  ) {}

  ngOnInit(): void {
    this.incomeService.getCategories().subscribe((categories: any) => {
      this.categories = categories;
      this.filteredCategories =categories;
    })
    this.initializeForm();
  }

  initializeForm(): void {

    this.incomeForm = this.fb.group({
      category: [null, Validators.required],
      amount: [
        null,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      date: [
        this.toDateOnly(new Date()),
        Validators.required
      ]
    });

  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
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
    this.incomeForm.patchValue({
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

    if (this.incomeForm.invalid) {
      this.incomeForm.markAllAsTouched();
      return;
    }

    const formValue = this.incomeForm.getRawValue();

    const income = {
      categoryId: formValue.category.id,
      category: formValue.category.name,
      amount: Number(formValue.amount),
      trxDate: new Date(formValue.date)
    };

    this.dialogRef.close(income);
  }

  close(): void {
    this.dialogRef.close();
  }
}

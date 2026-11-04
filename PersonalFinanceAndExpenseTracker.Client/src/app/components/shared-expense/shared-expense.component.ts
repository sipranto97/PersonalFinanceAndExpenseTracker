import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { SharedExpenseHistory } from '../../models/expense-share.model';
import { ExpenseShareService } from '../../services/expense-share.service';

@Component({
    selector: 'app-shared-expense',
    templateUrl: './shared-expense.component.html',
    styleUrl: './shared-expense.component.css',
    standalone: false
})
export class SharedExpenseComponent implements OnInit {
  isLoading = true;
  errorMessage: string | null = null;
  history: SharedExpenseHistory | null = null;

  constructor(
    private route: ActivatedRoute,
    private expenseShareService: ExpenseShareService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.paramMap.get('token');

    if (!token) {
      this.isLoading = false;
      this.errorMessage = 'This link is invalid.';
      return;
    }

    this.expenseShareService.getSharedExpenseHistory(token).subscribe({
      next: (history: SharedExpenseHistory) => {
        this.isLoading = false;
        this.history = history;
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading = false;

        if (err.status === 410) {
          this.errorMessage = 'This share link has expired.';
        } else if (err.status === 404) {
          this.errorMessage = "This share link doesn't exist.";
        } else {
          this.errorMessage = 'Something went wrong while loading this page.';
        }
      }
    });
  }
}

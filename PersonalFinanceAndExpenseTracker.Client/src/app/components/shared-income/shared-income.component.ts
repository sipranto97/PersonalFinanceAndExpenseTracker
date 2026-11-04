import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { SharedIncomeHistory } from '../../models/income-share.model';
import { IncomeShareService } from '../../services/income-share.service';

@Component({
    selector: 'app-shared-income',
    templateUrl: './shared-income.component.html',
    styleUrl: './shared-income.component.css',
    standalone: false
})
export class SharedIncomeComponent implements OnInit {
  isLoading = true;
  errorMessage: string | null = null;
  history: SharedIncomeHistory | null = null;

  constructor(
    private route: ActivatedRoute,
    private incomeShareService: IncomeShareService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.paramMap.get('token');

    if (!token) {
      this.isLoading = false;
      this.errorMessage = 'This link is invalid.';
      return;
    }

    this.incomeShareService.getSharedIncomeHistory(token).subscribe({
      next: (history: SharedIncomeHistory) => {
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

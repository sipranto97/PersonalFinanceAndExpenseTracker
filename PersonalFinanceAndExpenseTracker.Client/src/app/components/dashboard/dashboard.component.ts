import { Component, OnDestroy, OnInit } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { DashboardService } from '../../services/dashboard.service';
import { SavingsGoalService } from '../../services/savings-goal.service';
import { AccountService } from '../../services/account.service';
import { CurrencyService } from '../../services/currency.service';
import { buildIncomeExpensePieConfig } from '../../utils/chart-config.util';
import { SUPPORTED_CURRENCIES } from '../../utils/currency.util';
import { SavingsGoal } from '../../models/savings-goal.model';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.css',
    standalone: false
})
export class DashboardComponent implements OnInit, OnDestroy {
  userEmail: string = "";
  userFullName: string = "";
  totalIncome: number = 0;
  totalExpense: number = 0;
  balance: number = 0;

  pieConfig: ChartConfiguration<any> | null = null;

  goals: SavingsGoal[] = [];

  currencies = SUPPORTED_CURRENCIES;
  selectedCurrency: string | null = null;
  isSavingCurrency = false;

  private currencySub?: Subscription;

  constructor(
    private dashboardService: DashboardService,
    private savingsGoalService: SavingsGoalService,
    private accountService: AccountService,
    private currencyService: CurrencyService,
    private toastr: ToastrService
  ){}

  // Shown once for accounts whose currency was auto-picked (e.g. first Google sign-in).
  get showCurrencyPrompt(): boolean {
    return this.currencyService.needsSetup;
  }

  get currencyCode(): string {
    return this.currencyService.code;
  }

  ngOnInit(): void {
    this.dashboardService.getUserInfo().subscribe(res => {
      this.userEmail = res.username;
      this.userFullName = res.fullName;
    });

    this.loadSummary();

    // Re-label the pie if the currency arrives late or changes.
    this.currencySub = this.currencyService.currency$.subscribe(() => this.buildPie());

    this.savingsGoalService.getActiveGoals().subscribe(res => {
      this.goals = res;
    });
  }

  ngOnDestroy(): void {
    this.currencySub?.unsubscribe();
  }

  loadSummary(): void {
    this.dashboardService.getBalance().subscribe(res => {
      this.totalIncome = res.totalIncome;
      this.totalExpense = res.totalExpense;
      this.balance = res.balance;

      this.buildPie();
    });
  }

  private buildPie(): void {
    this.pieConfig = (this.totalIncome + this.totalExpense) > 0
      ? buildIncomeExpensePieConfig(this.totalIncome, this.totalExpense, this.currencyService.code)
      : null;
  }

  confirmCurrency(): void {
    const code = this.selectedCurrency ?? this.currencyService.code;

    this.isSavingCurrency = true;

    this.accountService.updateProfile({ currency: code }).subscribe({
      next: () => {
        this.isSavingCurrency = false;
        this.currencyService.set(code);
        this.toastr.success('Currency saved. You can change it any time from your account.');
      },
      error: (err) => {
        this.isSavingCurrency = false;
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to save currency.');
      }
    });
  }
}

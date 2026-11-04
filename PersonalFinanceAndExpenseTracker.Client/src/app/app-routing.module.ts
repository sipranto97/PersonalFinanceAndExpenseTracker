import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SignInComponent } from './components/sign-in/sign-in.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { IncomeComponent } from './components/income/income.component';
import { RecurringIncomeComponent } from './components/recurring-income/recurring-income.component';
import { SharedIncomeComponent } from './components/shared-income/shared-income.component';
import { ExpenseComponent } from './components/expense/expense.component';
import { RecurringExpenseComponent } from './components/recurring-expense/recurring-expense.component';
import { SharedExpenseComponent } from './components/shared-expense/shared-expense.component';
import { SavingsGoalComponent } from './components/savings-goal/savings-goal.component';
import { AccountComponent } from './components/account/account.component';
import { SharedChartComponent } from './components/shared-chart/shared-chart.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

const routes: Routes = [
  { path: '', redirectTo: '/sign-in', pathMatch: 'full' },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard],
    data: { title: 'Dashboard' },
  },
  {
    path: 'sign-in',
    component: SignInComponent,
    canActivate: [guestGuard],
    data: { title: 'Sign In' },
  },
  {
    path: 'register',
    component: SignUpComponent,
    canActivate: [guestGuard],
    data: { title: 'Sign Up' },
  },
  {
    path: 'signin-google',
    loadComponent: () =>
      import('./components/google-callback/google-callback.component').then(
        (m) => m.GoogleCallbackComponent
      ),
  },
  {
    path: 'income',
    component: IncomeComponent,
    canActivate: [authGuard],
    data: { title: 'Income' },
  },
  {
    path: 'income/recurring',
    component: RecurringIncomeComponent,
    canActivate: [authGuard],
    data: { title: 'Recurring Income' },
  },
  {
    path: 'share/income/:token',
    component: SharedIncomeComponent,
    data: { title: 'Shared Income History' },
  },
  {
    path: 'expense',
    component: ExpenseComponent,
    canActivate: [authGuard],
    data: { title: 'Expense' },
  },
  {
    path: 'expense/recurring',
    component: RecurringExpenseComponent,
    canActivate: [authGuard],
    data: { title: 'Recurring Expense' },
  },
  {
    path: 'share/expense/:token',
    component: SharedExpenseComponent,
    data: { title: 'Shared Expense History' },
  },
  {
    path: 'goals',
    component: SavingsGoalComponent,
    canActivate: [authGuard],
    data: { title: 'Savings Goals' },
  },
  {
    path: 'account',
    component: AccountComponent,
    canActivate: [authGuard],
    data: { title: 'User Account' },
  },
  {
    path: 'share/chart/:token',
    component: SharedChartComponent,
    data: { title: 'Shared Chart' },
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
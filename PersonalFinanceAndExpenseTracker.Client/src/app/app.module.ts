import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignInButtonGoogleComponent } from './components/sign-in-button-google/sign-in-button-google.component';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { SignInComponent } from './components/sign-in/sign-in.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { AuthorizeInterceptor } from './services/authorize-interceptor.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { OAuthModule } from 'angular-oauth2-oidc';
import { GoogleCallbackComponent } from './components/google-callback/google-callback.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { IncomeComponent } from './components/income/income.component';
import { AddIncomeModalComponent } from './components/add-income-modal/add-income-modal.component';
import { ExportIncomeModalComponent } from './components/export-income-modal/export-income-modal.component';
import { RecurringIncomeComponent } from './components/recurring-income/recurring-income.component';
import { AddRecurringIncomeModalComponent } from './components/add-recurring-income-modal/add-recurring-income-modal.component';
import { ShareIncomeModalComponent } from './components/share-income-modal/share-income-modal.component';
import { SharedIncomeComponent } from './components/shared-income/shared-income.component';
import { ExpenseComponent } from './components/expense/expense.component';
import { AddExpenseModalComponent } from './components/add-expense-modal/add-expense-modal.component';
import { ExportExpenseModalComponent } from './components/export-expense-modal/export-expense-modal.component';
import { ShareExpenseModalComponent } from './components/share-expense-modal/share-expense-modal.component';
import { RecurringExpenseComponent } from './components/recurring-expense/recurring-expense.component';
import { AddRecurringExpenseModalComponent } from './components/add-recurring-expense-modal/add-recurring-expense-modal.component';
import { SharedExpenseComponent } from './components/shared-expense/shared-expense.component';
import { SavingsGoalComponent } from './components/savings-goal/savings-goal.component';
import { AddSavingsGoalModalComponent } from './components/add-savings-goal-modal/add-savings-goal-modal.component';
import { AccountComponent } from './components/account/account.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NgSelectModule } from '@ng-select/ng-select';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { ShareChartModalComponent } from './components/share-chart-modal/share-chart-modal.component';
import { SharedChartComponent } from './components/shared-chart/shared-chart.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInButtonGoogleComponent,
    SignInComponent,
    DashboardComponent,
    SignUpComponent,
    SidebarComponent,
    IncomeComponent,
    AddIncomeModalComponent,
    ExportIncomeModalComponent,
    RecurringIncomeComponent,
    AddRecurringIncomeModalComponent,
    ShareIncomeModalComponent,
    SharedIncomeComponent,
    ExpenseComponent,
    AddExpenseModalComponent,
    ExportExpenseModalComponent,
    ShareExpenseModalComponent,
    RecurringExpenseComponent,
    AddRecurringExpenseModalComponent,
    SharedExpenseComponent,
    SavingsGoalComponent,
    AddSavingsGoalModalComponent,
    AccountComponent,
    PaginationComponent,
    ShareChartModalComponent,
    SharedChartComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    ToastrModule.forRoot({
      timeOut: 5000,
      positionClass: "toast-top-right",
      preventDuplicates: false,
      closeButton: true,
      newestOnTop: true,
      autoDismiss: true
    }),
    BrowserAnimationsModule,
    OAuthModule.forRoot(),
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    NgSelectModule,
    FormsModule,
    BaseChartDirective
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
    provideCharts(withDefaultRegisterables())
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ChartConfiguration } from 'chart.js';
import { Observable } from 'rxjs';
import { DashboardService } from '../../services/dashboard.service';
import { SavingsGoalService } from '../../services/savings-goal.service';
import { DATE_RANGE_OPTIONS, DateRangeKey, resolveDateRange } from '../../utils/date-range.util';
import { buildExpenseChartConfig, buildIncomeChartConfig, buildOverviewChartConfig } from '../../utils/chart-config.util';
import { ShareChartModalComponent } from '../share-chart-modal/share-chart-modal.component';
import { ChartType } from '../../models/dashboard-chart.model';
import { SavingsGoal } from '../../models/savings-goal.model';

interface ChartSection {
  chartType: ChartType;
  title: string;
  range: DateRangeKey;
  customStart: string;
  customEnd: string;
  startDate: string;
  endDate: string;
  config: ChartConfiguration<any> | null;
  fetch: (startDate: string, endDate: string) => Observable<any>;
  buildConfig: (data: any) => ChartConfiguration<any>;
}

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.css',
    standalone: false
})
export class DashboardComponent implements OnInit {
  userEmail: string = "";
  userFullName: string = "";
  totalIncome: number = 0;
  totalExpense: number = 0;
  balance: number = 0;

  rangeOptions = DATE_RANGE_OPTIONS;

  overview: ChartSection = {
    chartType: 'Overview',
    title: 'Financial Overview',
    range: '30d',
    customStart: '',
    customEnd: '',
    startDate: '',
    endDate: '',
    config: null,
    fetch: (startDate, endDate) => this.dashboardService.getOverviewChart(startDate, endDate),
    buildConfig: (data) => buildOverviewChartConfig(data)
  };

  expense: ChartSection = {
    chartType: 'Expense',
    title: 'Last 30 Days Expenses',
    range: '30d',
    customStart: '',
    customEnd: '',
    startDate: '',
    endDate: '',
    config: null,
    fetch: (startDate, endDate) => this.dashboardService.getExpenseChart(startDate, endDate),
    buildConfig: (data) => buildExpenseChartConfig(data)
  };

  income: ChartSection = {
    chartType: 'Income',
    title: 'Last 60 Days Income',
    range: '60d',
    customStart: '',
    customEnd: '',
    startDate: '',
    endDate: '',
    config: null,
    fetch: (startDate, endDate) => this.dashboardService.getIncomeChart(startDate, endDate),
    buildConfig: (data) => buildIncomeChartConfig(data)
  };

  sections: ChartSection[] = [this.overview, this.expense, this.income];

  goals: SavingsGoal[] = [];

  constructor(
    private router: Router,
    private dashboardService: DashboardService,
    private savingsGoalService: SavingsGoalService,
    private dialog: MatDialog
  ){}

  ngOnInit(): void {
    this.dashboardService.getUserInfo().subscribe(res => {
      this.userEmail = res.username;
      this.userFullName = res.fullName;
    });

    this.loadSummary();
    this.sections.forEach(section => this.loadSection(section));

    this.savingsGoalService.getActiveGoals().subscribe(res => {
      this.goals = res;
    });
  }

  onRangeChange(section: ChartSection): void {
    if (section.range === 'custom' && (!section.customStart || !section.customEnd)) {
      return;
    }

    this.loadSection(section);
  }

  loadSection(section: ChartSection): void {
    const { startDate, endDate } = resolveDateRange(section.range, {
      startDate: section.customStart,
      endDate: section.customEnd
    });

    section.startDate = startDate;
    section.endDate = endDate;

    section.fetch(startDate, endDate).subscribe(data => {
      section.config = section.buildConfig(data);
    });
  }

  loadSummary(): void {
    this.dashboardService.getBalance().subscribe(res => {
      this.totalIncome = res.totalIncome;
      this.totalExpense = res.totalExpense;
      this.balance = res.balance;
    });
  }

  openShareModal(section: ChartSection): void {
    this.dialog.open(ShareChartModalComponent, {
      width: '480px',
      disableClose: true,
      data: {
        chartType: section.chartType,
        startDate: section.startDate,
        endDate: section.endDate
      }
    });
  }
}

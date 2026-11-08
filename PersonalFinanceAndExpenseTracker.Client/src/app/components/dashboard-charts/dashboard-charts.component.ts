import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ChartConfiguration } from 'chart.js';
import { Observable, Subscription } from 'rxjs';
import { DashboardService } from '../../services/dashboard.service';
import { CurrencyService } from '../../services/currency.service';
import { DATE_RANGE_OPTIONS, DateRangeKey, resolveDateRange } from '../../utils/date-range.util';
import { buildExpenseChartConfig, buildIncomeChartConfig, buildOverviewChartConfig } from '../../utils/chart-config.util';
import { ShareChartModalComponent } from '../share-chart-modal/share-chart-modal.component';
import { ChartType } from '../../models/dashboard-chart.model';

interface ChartSection {
  chartType: ChartType;
  title: string;
  range: DateRangeKey;
  customStart: string;
  customEnd: string;
  startDate: string;
  endDate: string;
  config: ChartConfiguration<any> | null;
  data: any;
  fetch: (startDate: string, endDate: string) => Observable<any>;
  buildConfig: (data: any, currency: string) => ChartConfiguration<any>;
}

@Component({
    selector: 'app-dashboard-charts',
    templateUrl: './dashboard-charts.component.html',
    styleUrl: './dashboard-charts.component.css',
    standalone: false
})
export class DashboardChartsComponent implements OnInit, OnDestroy {
  rangeOptions = DATE_RANGE_OPTIONS;

  private currencySub?: Subscription;

  overview: ChartSection = {
    chartType: 'Overview',
    title: 'Financial Overview',
    range: '30d',
    customStart: '',
    customEnd: '',
    startDate: '',
    endDate: '',
    config: null,
    data: null,
    fetch: (startDate, endDate) => this.dashboardService.getOverviewChart(startDate, endDate),
    buildConfig: (data, currency) => buildOverviewChartConfig(data, currency)
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
    data: null,
    fetch: (startDate, endDate) => this.dashboardService.getExpenseChart(startDate, endDate),
    buildConfig: (data, currency) => buildExpenseChartConfig(data, currency)
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
    data: null,
    fetch: (startDate, endDate) => this.dashboardService.getIncomeChart(startDate, endDate),
    buildConfig: (data, currency) => buildIncomeChartConfig(data, currency)
  };

  sections: ChartSection[] = [this.overview, this.expense, this.income];

  constructor(
    private dashboardService: DashboardService,
    private currencyService: CurrencyService,
    private dialog: MatDialog
  ){}

  ngOnInit(): void {
    this.sections.forEach(section => this.loadSection(section));

    // Re-label the charts if the currency arrives late or changes.
    this.currencySub = this.currencyService.currency$.subscribe(() => {
      this.sections.forEach(section => {
        if (section.data) {
          section.config = section.buildConfig(section.data, this.currencyService.code);
        }
      });
    });
  }

  ngOnDestroy(): void {
    this.currencySub?.unsubscribe();
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
      section.data = data;
      section.config = section.buildConfig(data, this.currencyService.code);
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

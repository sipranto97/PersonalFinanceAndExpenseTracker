import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ChartConfiguration } from 'chart.js';
import { SharedChartData } from '../../models/dashboard-chart.model';
import { ChartShareService } from '../../services/chart-share.service';
import { buildExpenseChartConfig, buildIncomeChartConfig, buildOverviewChartConfig } from '../../utils/chart-config.util';

@Component({
    selector: 'app-shared-chart',
    templateUrl: './shared-chart.component.html',
    styleUrl: './shared-chart.component.css',
    standalone: false
})
export class SharedChartComponent implements OnInit {
  isLoading = true;
  errorMessage: string | null = null;
  data: SharedChartData | null = null;
  chartConfig: ChartConfiguration<any> | null = null;

  constructor(
    private route: ActivatedRoute,
    private chartShareService: ChartShareService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.paramMap.get('token');

    if (!token) {
      this.isLoading = false;
      this.errorMessage = 'This link is invalid.';
      return;
    }

    this.chartShareService.getSharedChartData(token).subscribe({
      next: (data: SharedChartData) => {
        this.isLoading = false;
        this.data = data;
        this.chartConfig = this.buildConfig(data);
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

  private buildConfig(data: SharedChartData): ChartConfiguration<any> | null {
    switch (data.chartType) {
      case 'Overview':
        return data.overview ? buildOverviewChartConfig(data.overview, data.currency) : null;
      case 'Expense':
        return data.expense ? buildExpenseChartConfig(data.expense, data.currency) : null;
      case 'Income':
        return data.income ? buildIncomeChartConfig(data.income, data.currency) : null;
    }
  }
}

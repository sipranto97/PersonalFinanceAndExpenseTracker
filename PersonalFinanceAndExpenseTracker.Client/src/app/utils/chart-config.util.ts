import { ChartConfiguration } from 'chart.js';
import { ChartSeriesPoint, OverviewChartPoint } from '../models/dashboard-chart.model';

function formatDateLabel(date: Date): string {
  return new Date(date).toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

export function buildOverviewChartConfig(points: OverviewChartPoint[]): ChartConfiguration<any> {
  const labels = points.map(p => formatDateLabel(p.date));

  return {
    type: 'bar',
    data: {
      labels,
      datasets: [
        {
          type: 'bar',
          label: 'Income',
          data: points.map(p => p.income),
          backgroundColor: '#16a34a',
        },
        {
          type: 'bar',
          label: 'Expense',
          data: points.map(p => p.expense),
          backgroundColor: '#dc2626',
        },
        {
          type: 'line',
          label: 'Balance',
          data: points.map(p => p.balance),
          borderColor: '#2563eb',
          backgroundColor: '#2563eb',
          tension: 0.3,
          fill: false,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      interaction: { mode: 'index', intersect: false },
      plugins: {
        tooltip: {
          callbacks: {
            label: (item: any) => `${item.dataset.label}: ${item.formattedValue}`,
          },
        },
      },
    },
  };
}

function buildSeriesChartConfig(
  points: ChartSeriesPoint[],
  label: string,
  color: string,
  type: 'bar' | 'line'
): ChartConfiguration<any> {
  const labels = points.map(p => formatDateLabel(p.date));

  return {
    type,
    data: {
      labels,
      datasets: [
        {
          label,
          data: points.map(p => p.amount),
          backgroundColor: color,
          borderColor: color,
          tension: 0.3,
          fill: type === 'line' ? false : undefined,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        tooltip: {
          callbacks: {
            afterBody: (items: any[]) => {
              const point = points[items[0]?.dataIndex];

              if (!point || !point.categoryBreakdown.length) {
                return [];
              }

              return point.categoryBreakdown.map(c => `${c.category}: ${c.amount}`);
            },
          },
        },
      },
    },
  };
}

export function buildExpenseChartConfig(points: ChartSeriesPoint[]): ChartConfiguration {
  return buildSeriesChartConfig(points, 'Expense', '#dc2626', 'bar');
}

export function buildIncomeChartConfig(points: ChartSeriesPoint[]): ChartConfiguration {
  return buildSeriesChartConfig(points, 'Income', '#16a34a', 'line');
}

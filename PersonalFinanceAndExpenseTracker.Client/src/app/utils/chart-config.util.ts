import { ChartConfiguration } from 'chart.js';
import { ChartSeriesPoint, OverviewChartPoint } from '../models/dashboard-chart.model';
import { DEFAULT_CURRENCY, formatMoney } from './currency.util';

// Y-axis tick labels in the user's currency, without cents to keep them short.
function currencyAxis(currency: string) {
  return {
    y: {
      ticks: {
        callback: (value: any) => formatMoney(value, currency, 0),
      },
    },
  };
}

function formatDateLabel(date: Date): string {
  return new Date(date).toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

export function buildIncomeExpensePieConfig(
  income: number,
  expense: number,
  currency: string = DEFAULT_CURRENCY
): ChartConfiguration<any> {
  const total = income + expense;
  const pct = (value: number) => (total > 0 ? Math.round((value / total) * 1000) / 10 : 0);

  // Draws each slice's percentage at the centre of that slice.
  const sliceLabelPlugin = {
    id: 'sliceLabels',
    afterDatasetsDraw(chart: any) {
      const ctx = chart.ctx;
      const values: number[] = chart.data.datasets[0].data;

      chart.getDatasetMeta(0).data.forEach((arc: any, i: number) => {
        if (!values[i]) {
          return;
        }

        const { x, y } = arc.tooltipPosition();
        ctx.save();
        ctx.fillStyle = '#ffffff';
        ctx.font = 'bold 18px sans-serif';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(`${pct(values[i])}%`, x, y);
        ctx.restore();
      });
    },
  };

  return {
    type: 'pie',
    plugins: [sliceLabelPlugin],
    data: {
      labels: ['Income', 'Expense'],
      datasets: [
        {
          data: [income, expense],
          backgroundColor: ['#16a34a', '#dc2626'],
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom' },
        tooltip: {
          callbacks: {
            label: (item: any) => `${item.label}: ${formatMoney(item.parsed, currency)} (${pct(item.parsed)}%)`,
          },
        },
      },
    },
  };
}

export function buildOverviewChartConfig(
  points: OverviewChartPoint[],
  currency: string = DEFAULT_CURRENCY
): ChartConfiguration<any> {
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
      scales: currencyAxis(currency),
      plugins: {
        tooltip: {
          callbacks: {
            label: (item: any) => `${item.dataset.label}: ${formatMoney(item.parsed.y, currency)}`,
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
  type: 'bar' | 'line',
  currency: string
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
      scales: currencyAxis(currency),
      plugins: {
        tooltip: {
          callbacks: {
            label: (item: any) => `${item.dataset.label}: ${formatMoney(item.parsed.y, currency)}`,
            afterBody: (items: any[]) => {
              const point = points[items[0]?.dataIndex];

              if (!point || !point.categoryBreakdown.length) {
                return [];
              }

              return point.categoryBreakdown.map(c => `${c.category}: ${formatMoney(c.amount, currency)}`);
            },
          },
        },
      },
    },
  };
}

export function buildExpenseChartConfig(
  points: ChartSeriesPoint[],
  currency: string = DEFAULT_CURRENCY
): ChartConfiguration {
  return buildSeriesChartConfig(points, 'Expense', '#dc2626', 'bar', currency);
}

export function buildIncomeChartConfig(
  points: ChartSeriesPoint[],
  currency: string = DEFAULT_CURRENCY
): ChartConfiguration {
  return buildSeriesChartConfig(points, 'Income', '#16a34a', 'line', currency);
}

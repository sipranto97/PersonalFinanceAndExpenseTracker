export type ChartType = 'Overview' | 'Expense' | 'Income';

export interface BalanceSummary {
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

export interface OverviewChartPoint {
  date: Date;
  income: number;
  expense: number;
  balance: number;
}

export interface CategoryAmount {
  category: string;
  amount: number;
}

export interface ChartSeriesPoint {
  date: Date;
  amount: number;
  categoryBreakdown: CategoryAmount[];
}

export interface CreateChartShareLinkPayload {
  chartType: ChartType;
  startDate: string;
  endDate: string;
  expiresAt: string | null;
}

export interface SharedChartData {
  ownerName: string;
  currency: string;
  chartType: ChartType;
  startDate: Date;
  endDate: Date;
  expiresAt: Date | null;
  overview: OverviewChartPoint[] | null;
  expense: ChartSeriesPoint[] | null;
  income: ChartSeriesPoint[] | null;
}

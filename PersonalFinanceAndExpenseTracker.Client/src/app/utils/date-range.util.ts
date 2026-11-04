export type DateRangeKey = '7d' | '30d' | '60d' | '90d' | 'thisMonth' | 'lastMonth' | 'custom';

export interface DateRangeOption {
  key: DateRangeKey;
  label: string;
}

export const DATE_RANGE_OPTIONS: DateRangeOption[] = [
  { key: '7d', label: 'Last 7 Days' },
  { key: '30d', label: 'Last 30 Days' },
  { key: '60d', label: 'Last 60 Days' },
  { key: '90d', label: 'Last 90 Days' },
  { key: 'thisMonth', label: 'This Month' },
  { key: 'lastMonth', label: 'Last Month' },
  { key: 'custom', label: 'Custom' },
];

export interface ResolvedDateRange {
  startDate: string;
  endDate: string;
}

function toIsoDate(date: Date): string {
  return date.toISOString().split('T')[0];
}

export function resolveDateRange(key: DateRangeKey, custom?: { startDate: string; endDate: string }): ResolvedDateRange {
  const today = new Date();

  switch (key) {
    case '7d':
    case '30d':
    case '60d':
    case '90d': {
      const days = key === '7d' ? 7 : key === '30d' ? 30 : key === '60d' ? 60 : 90;
      const start = new Date(today);
      start.setDate(start.getDate() - (days - 1));
      return { startDate: toIsoDate(start), endDate: toIsoDate(today) };
    }
    case 'thisMonth': {
      const start = new Date(today.getFullYear(), today.getMonth(), 1);
      return { startDate: toIsoDate(start), endDate: toIsoDate(today) };
    }
    case 'lastMonth': {
      const start = new Date(today.getFullYear(), today.getMonth() - 1, 1);
      const end = new Date(today.getFullYear(), today.getMonth(), 0);
      return { startDate: toIsoDate(start), endDate: toIsoDate(end) };
    }
    case 'custom':
      if (!custom) {
        throw new Error('Custom date range requires startDate and endDate.');
      }
      return { startDate: custom.startDate, endDate: custom.endDate };
  }
}

export interface CurrencyOption {
  code: string;
  name: string;
}

export const DEFAULT_CURRENCY = 'USD';

// Keep in sync with SupportedCurrencies.cs on the API.
export const SUPPORTED_CURRENCIES: CurrencyOption[] = [
  { code: 'USD', name: 'US Dollar' },
  { code: 'EUR', name: 'Euro' },
  { code: 'GBP', name: 'British Pound' },
  { code: 'BDT', name: 'Bangladeshi Taka' },
  { code: 'INR', name: 'Indian Rupee' },
  { code: 'PKR', name: 'Pakistani Rupee' },
  { code: 'LKR', name: 'Sri Lankan Rupee' },
  { code: 'NPR', name: 'Nepalese Rupee' },
  { code: 'JPY', name: 'Japanese Yen' },
  { code: 'CNY', name: 'Chinese Yuan' },
  { code: 'KRW', name: 'South Korean Won' },
  { code: 'SGD', name: 'Singapore Dollar' },
  { code: 'MYR', name: 'Malaysian Ringgit' },
  { code: 'IDR', name: 'Indonesian Rupiah' },
  { code: 'THB', name: 'Thai Baht' },
  { code: 'PHP', name: 'Philippine Peso' },
  { code: 'VND', name: 'Vietnamese Dong' },
  { code: 'AED', name: 'UAE Dirham' },
  { code: 'SAR', name: 'Saudi Riyal' },
  { code: 'QAR', name: 'Qatari Riyal' },
  { code: 'KWD', name: 'Kuwaiti Dinar' },
  { code: 'TRY', name: 'Turkish Lira' },
  { code: 'AUD', name: 'Australian Dollar' },
  { code: 'NZD', name: 'New Zealand Dollar' },
  { code: 'CAD', name: 'Canadian Dollar' },
  { code: 'CHF', name: 'Swiss Franc' },
  { code: 'SEK', name: 'Swedish Krona' },
  { code: 'NOK', name: 'Norwegian Krone' },
  { code: 'DKK', name: 'Danish Krone' },
  { code: 'BRL', name: 'Brazilian Real' },
  { code: 'MXN', name: 'Mexican Peso' },
  { code: 'ZAR', name: 'South African Rand' },
  { code: 'NGN', name: 'Nigerian Naira' },
  { code: 'EGP', name: 'Egyptian Pound' },
  { code: 'KES', name: 'Kenyan Shilling' }
];

const EURO_REGIONS = [
  'AT', 'BE', 'CY', 'EE', 'FI', 'FR', 'DE', 'GR', 'IE', 'IT',
  'LV', 'LT', 'LU', 'MT', 'NL', 'PT', 'SK', 'SI', 'ES', 'HR'
];

const REGION_CURRENCY: Record<string, string> = {
  US: 'USD', GB: 'GBP', BD: 'BDT', IN: 'INR', PK: 'PKR', LK: 'LKR', NP: 'NPR',
  JP: 'JPY', CN: 'CNY', KR: 'KRW', SG: 'SGD', MY: 'MYR', ID: 'IDR', TH: 'THB',
  PH: 'PHP', VN: 'VND', AE: 'AED', SA: 'SAR', QA: 'QAR', KW: 'KWD', TR: 'TRY',
  AU: 'AUD', NZ: 'NZD', CA: 'CAD', CH: 'CHF', SE: 'SEK', NO: 'NOK', DK: 'DKK',
  BR: 'BRL', MX: 'MXN', ZA: 'ZAR', NG: 'NGN', EG: 'EGP', KE: 'KES'
};

EURO_REGIONS.forEach(region => (REGION_CURRENCY[region] = 'EUR'));

/** The browser's preferred locale, e.g. "en-US" or "bn-BD". */
export function getUserLocale(): string {
  return (typeof navigator !== 'undefined' && navigator.language) || 'en-US';
}

export function isSupportedCurrency(code: string | null | undefined): boolean {
  return !!code && SUPPORTED_CURRENCIES.some(c => c.code === code.toUpperCase());
}

/**
 * Best-guess currency for a locale. `Intl.Locale.maximize()` fills in a likely
 * region for language-only locales (e.g. "bn" -> BD), so this still works when
 * the browser reports no region.
 */
export function detectCurrency(locale: string = getUserLocale()): string {
  try {
    const region = new Intl.Locale(locale).maximize().region;
    const currency = region ? REGION_CURRENCY[region] : undefined;

    return currency && isSupportedCurrency(currency) ? currency : DEFAULT_CURRENCY;
  } catch {
    return DEFAULT_CURRENCY;
  }
}

const formatterCache = new Map<string, Intl.NumberFormat>();

function getFormatter(locale: string, currency: string, maximumFractionDigits?: number): Intl.NumberFormat {
  const key = `${locale}|${currency}|${maximumFractionDigits ?? ''}`;
  let formatter = formatterCache.get(key);

  if (!formatter) {
    formatter = new Intl.NumberFormat(locale, {
      style: 'currency',
      currency,
      ...(maximumFractionDigits !== undefined ? { maximumFractionDigits } : {})
    });
    formatterCache.set(key, formatter);
  }

  return formatter;
}

/**
 * Locale-aware money formatting. Display-only: the number is never converted,
 * only the symbol, grouping and decimal style change.
 */
export function formatMoney(
  value: number | string | null | undefined,
  currency: string = DEFAULT_CURRENCY,
  maximumFractionDigits?: number
): string {
  const amount = Number(value ?? 0);

  try {
    return getFormatter(getUserLocale(), currency, maximumFractionDigits).format(amount);
  } catch {
    return `${currency} ${amount}`;
  }
}

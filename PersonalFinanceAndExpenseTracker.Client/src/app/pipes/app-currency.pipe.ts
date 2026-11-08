import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyService } from '../services/currency.service';

/**
 * {{ amount | appCurrency }}            -> signed-in user's currency
 * {{ amount | appCurrency: ownerCode }} -> explicit currency (shared pages)
 *
 * Impure on purpose: it must re-render when the user's currency changes
 * without every list being re-fetched. Formatters are cached, so it is cheap.
 */
@Pipe({
  name: 'appCurrency',
  pure: false,
  standalone: false
})
export class AppCurrencyPipe implements PipeTransform {
  constructor(private currencyService: CurrencyService) {}

  transform(value: number | string | null | undefined, code?: string | null): string {
    return this.currencyService.format(value, code);
  }
}

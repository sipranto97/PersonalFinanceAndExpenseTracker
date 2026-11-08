import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AccountService } from './account.service';
import { DEFAULT_CURRENCY, detectCurrency, formatMoney, isSupportedCurrency } from '../utils/currency.util';

/**
 * Holds the signed-in user's currency. Until the account loads it falls back to
 * a guess from the browser locale, so amounts never render without a symbol.
 */
@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private codeSubject = new BehaviorSubject<string>(detectCurrency());
  currency$ = this.codeSubject.asObservable();

  /** True when the currency was auto-picked (e.g. Google sign-up) and not yet confirmed. */
  needsSetup = false;

  private loaded = false;
  private loading = false;

  constructor(private accountService: AccountService) {}

  get code(): string {
    return this.codeSubject.value;
  }

  ensureLoaded(): void {
    if (this.loaded || this.loading) {
      return;
    }

    this.loading = true;

    this.accountService.getAccount().subscribe({
      next: res => {
        const code = isSupportedCurrency(res.currency) ? res.currency : DEFAULT_CURRENCY;
        this.codeSubject.next(code);
        this.needsSetup = !!res.needsCurrencySetup;
        this.loaded = true;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  /** Called after the user saves a new currency (or confirms the suggested one). */
  set(code: string): void {
    this.codeSubject.next(code);
    this.needsSetup = false;
    this.loaded = true;
  }

  /** Called on sign-out so the next user doesn't inherit this one's currency. */
  reset(): void {
    this.loaded = false;
    this.loading = false;
    this.needsSetup = false;
    this.codeSubject.next(detectCurrency());
  }

  format(value: number | string | null | undefined, code?: string | null): string {
    return formatMoney(value, code || this.code);
  }
}

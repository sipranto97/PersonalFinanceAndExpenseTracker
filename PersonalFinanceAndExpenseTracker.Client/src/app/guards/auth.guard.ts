import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CurrencyService } from '../services/currency.service';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const currencyService = inject(CurrencyService);

  if (authService.isAuthenticated()) {
    // Fire-and-forget: pages render with a locale-based guess until it arrives.
    currencyService.ensureLoaded();
    return true;
  }

  authService.clearSession();
  router.navigate(['/sign-in']);
  return false;
};

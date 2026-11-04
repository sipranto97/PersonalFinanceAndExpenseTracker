import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from './services/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: false
})
export class AppComponent implements OnInit {
  title = 'PersonalFinanceAndExpenseTracker.Client';
  hideChrome = false;

  constructor(private router: Router, private authService: AuthService) {}

  private static readonly noChromeRoutes = ['/share', '/sign-in', '/register', '/signin-google'];

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.authService.fetchCsrfToken().subscribe({ error: () => {} });
    }

    this.hideChrome = AppComponent.shouldHideChrome(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(event => {
        this.hideChrome = AppComponent.shouldHideChrome(event.urlAfterRedirects);
      });
  }

  private static shouldHideChrome(url: string): boolean {
    return AppComponent.noChromeRoutes.some(route => url.startsWith(route));
  }
}

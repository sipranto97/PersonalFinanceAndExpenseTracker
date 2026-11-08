import { Component, OnInit } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
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

  /** Phones and small tablets: sidebar becomes a drawer behind a top-bar button. */
  isMobile = false;
  mobileNavOpen = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    private breakpointObserver: BreakpointObserver
  ) {}

  private static readonly noChromeRoutes = ['/share', '/sign-in', '/register', '/signin-google'];

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.authService.fetchCsrfToken().subscribe({ error: () => {} });
    }

    this.hideChrome = AppComponent.shouldHideChrome(this.router.url);

    this.breakpointObserver.observe('(max-width: 768px)').subscribe(state => {
      this.isMobile = state.matches;

      if (!this.isMobile) {
        this.mobileNavOpen = false;
      }
    });

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(event => {
        this.hideChrome = AppComponent.shouldHideChrome(event.urlAfterRedirects);
        this.mobileNavOpen = false;
      });
  }

  private static shouldHideChrome(url: string): boolean {
    return AppComponent.noChromeRoutes.some(route => url.startsWith(route));
  }
}

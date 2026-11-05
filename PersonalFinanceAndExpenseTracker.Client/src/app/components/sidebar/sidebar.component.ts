import { environment } from '../../../environments/environment';
import { Component, OnInit } from '@angular/core';
import { MenuItem } from '../../models/menuitem.model';
import { Router } from '@angular/router';
import { AccountService } from '../../services/account.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.css',
    standalone: false
})
export class SidebarComponent implements OnInit {
  private readonly backendOrigin = environment.apiOrigin;

  isCollapsed = false;
  avatarUrl: string | null = null;
  fullName: string | null = null;

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'fas fa-home',
      route: '/dashboard'
    },
    {
      label: 'Income',
      icon: 'fas fa-wallet',
      route: '/income'
    },
    {
      label: 'Expense',
      icon: 'fas fa-money-bill-wave',
      route: '/expense'
    },
    {
      label: 'Goals',
      icon: 'fas fa-bullseye',
      route: '/goals'
    },
    {
      label: 'User Account',
      icon: 'fas fa-user-circle',
      route: '/account'
    }
  ];

  constructor(
    public router: Router,
    private accountService: AccountService,
    private authService: AuthService
  ){}

  ngOnInit(): void {
    if (!this.isLoggedIn()) {
      return;
    }

    this.accountService.getAccount().subscribe(res => {
      this.fullName = res.fullName || res.username;
      this.avatarUrl = this.resolveAvatarUrl(res.profilePictureUrl);
    });

    this.accountService.profileUpdated$.subscribe(change => {
      if (change.fullName !== undefined) {
        this.fullName = change.fullName;
      }

      if (change.profilePictureUrl !== undefined) {
        this.avatarUrl = this.resolveAvatarUrl(change.profilePictureUrl);
      }
    });
  }

  private resolveAvatarUrl(url: string | null | undefined): string | null {
    if (!url) {
      return null;
    }

    return url.startsWith('/') ? `${this.backendOrigin}${url}` : url;
  }

  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  isLoggedIn() {
    return this.authService.isAuthenticated();
  }

  logout() {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(["/sign-in"]),
      error: () => this.router.navigate(["/sign-in"])
    });
  }

}

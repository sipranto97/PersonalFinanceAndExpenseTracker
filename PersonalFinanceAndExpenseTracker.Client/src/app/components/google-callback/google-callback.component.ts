import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-google-callback',
    imports: [CommonModule],
    templateUrl: './google-callback.component.html',
    styleUrl: './google-callback.component.css'
})
export class GoogleCallbackComponent {
  constructor(private auth: AuthService, private router: Router) {}

  async ngOnInit() {

     const result =  await this.auth.processGoogleLogin();

      if (result) {
        result.subscribe(() => {
          this.router.navigate(['/dashboard']);
        });
      } else {
        this.router.navigate(['/sign-in']);
      }
  }
}

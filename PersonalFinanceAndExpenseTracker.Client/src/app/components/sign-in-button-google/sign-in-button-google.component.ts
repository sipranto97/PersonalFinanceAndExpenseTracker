import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-sign-in-button-google',
    templateUrl: './sign-in-button-google.component.html',
    styleUrl: './sign-in-button-google.component.css',
    standalone: false
})
export class SignInButtonGoogleComponent {
  @Input() variant: 'light' | 'dark' = 'light';
  @Input() label = 'Sign in with Google';
  @Output() clicked = new EventEmitter<void>();

  isLoading = false;

  onClick(): void {
    if (this.isLoading) return;
    this.isLoading = true;
    this.clicked.emit();
  }

  resetLoading(): void {
    this.isLoading = false;
  }
}

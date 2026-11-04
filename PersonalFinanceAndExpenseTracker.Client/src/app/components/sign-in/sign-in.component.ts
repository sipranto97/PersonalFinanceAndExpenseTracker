import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginResponseModel } from '../../modals/login-response.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrl: './sign-in.component.css',
    standalone: false
})
export class SignInComponent implements OnInit{
  showPassword = false;
  isLoading = false;

  loginForm: any;

  constructor(
    private fb: FormBuilder, 
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      remember: [false]
    });
  }

  ngOnInit(): void {
  }

  isInvalid(field: string): boolean {
    const control = this.loginForm.get(field);
    return !!(control?.invalid && control?.touched);
  }

  onSubmit(): void {
    this.loginForm.markAllAsTouched();

    if (this.loginForm.invalid) {
       return;
    }

    this.isLoading = true;
    const payload = {
      username: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.login(payload).subscribe((res: LoginResponseModel) => {
      this.toastr.success("Login Successfull");
      this.router.navigate(['/dashboard']);
    },
    (err: HttpErrorResponse) => {
      this.isLoading = false;
      this.toastr.success("Invalid username or password");
    }
    )
  }

  loginWithGoogle(): void {
    // trigger Google OAuth flow
    this.authService.googleLogin();
  }
}

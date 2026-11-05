import { environment } from '../../../environments/environment';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../services/account.service';

@Component({
    selector: 'app-account',
    templateUrl: './account.component.html',
    styleUrl: './account.component.css',
    standalone: false
})
export class AccountComponent implements OnInit {
  private readonly backendOrigin = environment.apiOrigin;

  username: string = '';
  hasPassword: boolean = true;
  profilePictureUrl: string | null = null;

  profileForm!: FormGroup;
  passwordForm!: FormGroup;

  isSavingProfile = false;
  isChangingPassword = false;
  isUploadingPicture = false;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.getAccount();
  }

  initializeForms(): void {
    this.profileForm = this.fb.group({
      fullName: ['', Validators.required]
    });

    this.passwordForm = this.fb.group(
      {
        currentPassword: ['', Validators.required],
        newPassword: ['', [Validators.required, Validators.minLength(8)]],
        confirmNewPassword: ['', Validators.required]
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword')?.value;
    const confirmNewPassword = control.get('confirmNewPassword')?.value;

    return newPassword === confirmNewPassword ? null : { passwordMismatch: true };
  }

  getAccount(): void {
    this.accountService.getAccount().subscribe(res => {
      this.username = res.username;
      this.hasPassword = res.hasPassword;
      this.profilePictureUrl = res.profilePictureUrl;
      this.profileForm.patchValue({ fullName: res.fullName });
    });
  }

  get avatarUrl(): string | null {
    if (!this.profilePictureUrl) {
      return null;
    }

    return this.profilePictureUrl.startsWith('/')
      ? `${this.backendOrigin}${this.profilePictureUrl}`
      : this.profilePictureUrl;
  }

  onPictureSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.toastr.error('Profile picture must be 5MB or smaller.');
      input.value = '';
      return;
    }

    this.isUploadingPicture = true;

    this.accountService.uploadProfilePicture(file).subscribe({
      next: (res) => {
        this.isUploadingPicture = false;
        this.profilePictureUrl = res.profilePictureUrl;
        this.toastr.success('Profile picture updated successfully.');
      },
      error: (err) => {
        this.isUploadingPicture = false;
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to upload profile picture.');
      }
    });

    input.value = '';
  }

  removePicture(): void {
    this.accountService.removeProfilePicture().subscribe({
      next: () => {
        this.profilePictureUrl = null;
        this.toastr.success('Profile picture removed.');
      },
      error: (err) => {
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to remove profile picture.');
      }
    });
  }

  isProfileInvalid(controlName: string): boolean {
    const control = this.profileForm.get(controlName);
    return !!(control && control.invalid && (control.touched || control.dirty));
  }

  isPasswordInvalid(controlName: string): boolean {
    const control = this.passwordForm.get(controlName);
    return !!(control && control.invalid && (control.touched || control.dirty));
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSavingProfile = true;

    this.accountService.updateProfile(this.profileForm.value).subscribe({
      next: () => {
        this.isSavingProfile = false;
        this.accountService.notifyProfileUpdated({
          fullName: this.profileForm.value.fullName,
          profilePictureUrl: this.profilePictureUrl
        });
        this.toastr.success('Profile updated successfully.');
      },
      error: (err) => {
        this.isSavingProfile = false;
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to update profile.');
      }
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.isChangingPassword = true;

    const payload = {
      currentPassword: this.passwordForm.value.currentPassword,
      newPassword: this.passwordForm.value.newPassword
    };

    this.accountService.changePassword(payload).subscribe({
      next: () => {
        this.isChangingPassword = false;
        this.toastr.success('Password changed successfully.');
        this.passwordForm.reset();
      },
      error: (err) => {
        this.isChangingPassword = false;
        this.toastr.error(typeof err.error === 'string' ? err.error : 'Unable to change password.');
      }
    });
  }
}

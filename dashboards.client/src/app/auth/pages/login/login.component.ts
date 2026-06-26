import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { LoginRequest, ResetPasswordRequest } from '../../models/login.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export   class LoginComponent {
  // Login form
  username = '';
  password = '';

  // Reset password form
  newPassword = '';
  confirmPassword = '';

  // State
  showResetForm = false;
  showForgotPassword = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(): void {
    if (!this.username || !this.password) {
      this.errorMessage = 'Please enter username and password.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const request: LoginRequest = {
      username: this.username,
      password: this.password
    };

    this.authService.login(request).subscribe({
      next: (response) => {
        this.loading = false;

        if (response.requiresPasswordReset) {
          // Old user - show reset form
          this.showResetForm = true;
          this.successMessage = '';
          this.errorMessage = 'Your password needs to be updated. Please set a new password.';
        } else {
          // Success - save session and navigate
          this.authService.saveSession(response);
          this.router.navigate(['/user-management/users']);
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Login failed. Please try again.';
      }
    });
  }

  onShowForgotPassword(): void {
    this.showForgotPassword = true;
    this.showResetForm = false;
    this.errorMessage = '';
    this.successMessage = '';
    this.newPassword = '';
    this.confirmPassword = '';
  }

  onResetPassword(): void {
    if (this.showForgotPassword && !this.username) {
      this.errorMessage = 'Please enter your username.';
      return;
    }

    if (!this.newPassword || !this.confirmPassword) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    if (this.newPassword.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const request: ResetPasswordRequest = {
      username: this.username,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword
    };

    this.authService.resetPassword(request).subscribe({
      next: (response) => {
        this.loading = false;
        this.authService.saveSession(response);
        this.router.navigate(['/user-management/users']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Password reset failed. Please try again.';
      }
    });
  }

  onBackToLogin(): void {
    this.showResetForm = false;
    this.showForgotPassword = false;
    this.errorMessage = '';
    this.successMessage = '';
    this.newPassword = '';
    this.confirmPassword = '';
  }
}

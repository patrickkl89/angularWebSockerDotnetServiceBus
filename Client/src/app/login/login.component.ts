import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { AuthService } from '../services/auth/auth.service';
import { TokenStorageService } from '../services/token-storage/token-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  emailFormControl = new FormControl('', [
    Validators.required,
    Validators.email,
  ]);
  passwordFormControl = new FormControl('', [Validators.required]);
  isLoggedIn = false;
  isLoginFailed = false;
  roles: string[] = [];
  errorMessage = '';

  constructor(
    private readonly authService: AuthService,
    private readonly tokenStorage: TokenStorageService
  ) {}

  ngOnInit(): void {}

  onSubmit(): void {
    if (!this.emailFormControl.valid || !this.passwordFormControl.valid) return;
    this.authService
      .login(this.emailFormControl.value!, this.passwordFormControl.value!)
      .subscribe({
        next: (data) => {
          this.tokenStorage.saveToken(data.token);
          this.tokenStorage.saveUser(data);

          this.isLoginFailed = false;
          this.isLoggedIn = true;
          this.roles = this.tokenStorage.getUser().roles;
          // this.reloadPage();
        },
        error: (err) => {
          this.errorMessage = err.error.message;
          this.isLoginFailed = true;
        },
      });
  }
}

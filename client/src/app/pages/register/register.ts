import { Component } from '@angular/core';
import {
  NonNullableFormBuilder,
  Validators,
  FormGroup,
  FormControl,
  AbstractControl,
  ValidationErrors,
  ReactiveFormsModule
} from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/AuthService/auth-service';

interface RegisterForm {
  name: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
  confirmPassword: FormControl<string>;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]
})
export class Register {
  form: FormGroup<RegisterForm>;
  errorMessage = '';

  constructor(
    private fb: NonNullableFormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: this.fb.control('', Validators.required),
      email: this.fb.control('', [Validators.required, Validators.email]),
      password: this.fb.control('', [Validators.required, Validators.minLength(6)]),
      confirmPassword: this.fb.control('', Validators.required)
    });


    this.form.controls.confirmPassword.addValidators(this.passwordMatchValidator.bind(this));
  }

  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = this.form?.controls.password.value;
    const confirm = control.value;
    return password === confirm ? null : { passwordMismatch: true };
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.errorMessage = 'Please fix all validation errors';
      return;
    }

    const registerDto = this.form.getRawValue();

    this.auth.register(registerDto).subscribe({
      next: () => {
        this.auth.refreshAuth();
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Registration failed';
      }
    });
  }
  navigateToLogin()
  {
    this.router.navigate(["login"])
  }
  get f() {
    return this.form.controls;
  }
}

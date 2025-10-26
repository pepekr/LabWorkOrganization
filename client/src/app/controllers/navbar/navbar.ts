import { Component } from '@angular/core';
import { AuthService } from '../../services/AuthService/auth-service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css'],
  standalone: true, // needed to use `imports` in Angular 15+
  imports: [CommonModule],
})
export class Navbar {
  auth$: Observable<{ isLoggedIn: boolean; email?: string }>;

  constructor(private auth: AuthService) {
    this.auth$ = this.auth.authorized$;
  }

  handleAuthClick(isLoggedIn: boolean) {
    if (isLoggedIn) {
      this.logout();
    } else {
      this.login();
    }
  }

  private login() {
    console.log('Executing login logic... 🚀');
    this.auth.refreshAuth();
  }
  private logout() {
    console.log('Executing logout logic... 👋');
    this.auth.refreshAuth();
  }
  handleExternalAuthClick(isLoggedIn: boolean) {
    if (isLoggedIn) {
      this.logoutExternal();
    } else {
      this.loginExternal();
    }
  }
  private logoutExternal() {}
  private loginExternal() {}
}

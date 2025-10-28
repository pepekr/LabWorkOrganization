
import { Component } from '@angular/core';
import { Observable, combineLatest, map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/AuthService/auth-service';
import { ExternalAuth } from '../../services/ExternalAuth/external-auth';

export interface AuthStatus {
  isLoggedInLocal: boolean;
  isLoggedInExternal: boolean;
  email?: string;
}
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css'],
  standalone: true,
  imports: [CommonModule],
})
export class Navbar {
  authAuthorized$: Observable<AuthStatus>;

  constructor(
    private auth: AuthService,
    private externalAuth: ExternalAuth,
    private router: Router
  ) {
    this.authAuthorized$ = combineLatest([
      this.auth.authorized$,
      this.externalAuth.authorized$
    ]).pipe(
      map(([local, external]) => ({
        isLoggedInLocal: local.isLoggedInLocal,
        isLoggedInExternal: external.isLoggedInExternal,
        email: local.email
      }))
    );
  }

  handleAuthClick(isLoggedInLocal: boolean) {
    isLoggedInLocal ? this.logout() : this.login();
  }

  private login() {
    this.router.navigate(['login']);
  }

  private logout() {
    this.auth.logout().subscribe(() => {
      this.auth.refreshAuth();
      this.externalAuth.refreshAuth();
      this.router.navigate(['login']);
    });
  }

  handleExternalAuthClick(isLoggedInExternal: boolean) {
    isLoggedInExternal ? this.logoutExternal() : this.loginExternal();
  }

  private logoutExternal() {
    this.externalAuth.handleExternalLogout().subscribe(() => {
      this.auth.refreshAuth();
      this.externalAuth.refreshAuth();
      this.router.navigate(['login']);
    });
  }

private loginExternal() {
  const returnUrl = this.router.url;
  this.externalAuth.loginWithGoogle(returnUrl);
}

}

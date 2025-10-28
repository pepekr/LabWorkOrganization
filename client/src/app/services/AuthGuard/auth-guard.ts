import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, take, map, tap } from 'rxjs/operators';
import { AuthService } from '../AuthService/auth-service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

 canActivate(): Observable<boolean | UrlTree> {
  // trigger a backend check when guard runs
  this.auth.refreshAuth();

  return this.auth.authorized$.pipe(
    filter(status => status.initialized === true),
    take(1),
    map(status =>
      status.isLoggedInLocal
        ? true
        : this.router.createUrlTree(['/login'])
    )
  );
}
}

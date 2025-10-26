import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { tap, take, map } from 'rxjs/operators';
import { AuthService } from "../AuthService/auth-service"

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

 canActivate(): Observable<boolean | UrlTree> {
  return this.auth.authorized$.pipe(
    take(1),
    map(auth => auth.isLoggedIn ? true : this.router.createUrlTree(['/login']))
  );
}
}

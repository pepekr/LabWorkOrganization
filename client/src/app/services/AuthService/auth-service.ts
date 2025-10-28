import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

export interface AuthStatus {
  isLoggedInLocal: boolean;
  isLoggedInExternal: boolean;
  email?: string;
  initialized: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _authorized$ = new BehaviorSubject<AuthStatus>({
    isLoggedInLocal: false,
    isLoggedInExternal: false,
    initialized: false
  });

  constructor(private http: HttpClient) {
    this.refreshAuth();
  }

  /** Observable to subscribe in guards/components */
  get authorized$(): Observable<AuthStatus> {
    return this._authorized$.asObservable();
  }

  /** Refresh login state from backend */
  refreshAuth(): void {
    this.http.get<{ isLoggedIn: boolean; email?: string }>(
      `${environment.backendUrl}/api/auth/isLoggedIn`,
      { withCredentials: true }
    ).pipe(
      tap(data => {
        const current = this._authorized$.value;
        this._authorized$.next({
          ...current,
          isLoggedInLocal: data.isLoggedIn,
          email: data.email,
          initialized: true
        });
      }),
      catchError(() => {
        const current = this._authorized$.value;
        this._authorized$.next({
          ...current,
          isLoggedInLocal: false,
          initialized: true
        });
        return of(null);
      })
    ).subscribe();
  }

  login(userLoginDto: { email: string, password: string }): Observable<any> {
    return this.http.post(`${environment.backendUrl}/api/auth/login`, userLoginDto, {
      withCredentials: true
    }).pipe(tap(() => this.refreshAuth()));
  }

  register(userRegisterDto: { name: string, email: string, password: string, confirmPassword: string }): Observable<any> {
    return this.http.post(`${environment.backendUrl}/api/auth/register`, userRegisterDto, {
      withCredentials: true
    }).pipe(tap(() => this.refreshAuth()));
  }

  logout(): Observable<any> {
    return this.http.post(`${environment.backendUrl}/api/auth/logout`, {}, {
      withCredentials: true
    }).pipe(tap(() => this.refreshAuth()));
  }
}

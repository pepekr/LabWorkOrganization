import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthStatus } from "../../components/navbar/navbar"

@Injectable({ providedIn: 'root' })
export class ExternalAuth {

  private _authorized$ = new BehaviorSubject<AuthStatus>({
    isLoggedInLocal: false,
    isLoggedInExternal: false
  });

  constructor(private http: HttpClient) {
    this.checkAuth();
  }

  private checkAuth() {
    this.http.get<boolean>(
      `${environment.backendUrl}/api/external-auth/isLoggedIn`,
      { withCredentials: true }
    ).subscribe({
      next: (isLoggedInExternal) => this._authorized$.next({
        ...this._authorized$.value,
        isLoggedInExternal
      }),
      error: () => this._authorized$.next({
        ...this._authorized$.value,
        isLoggedInExternal: false
      })
    });
  }

  get authorized$(): Observable<AuthStatus> {
    return this._authorized$.asObservable();
  }

  refreshAuth() {
    this.checkAuth();
  }

  loginWithGoogle(returnUrl: string = '/') {
    window.location.href =
      `${environment.backendUrl}/api/external-auth/external-login?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  handleExternalCallback(code: string): Observable<any> {
    return this.http.get(
      `${environment.backendUrl}/api/external-auth/external-callback`,
      {
        params: { code },
        withCredentials: true
      }
    );
  }

  handleExternalLogout(): Observable<any> {
    return this.http.post(
      `${environment.backendUrl}/api/external-auth/external-logout`,
      {},
      { withCredentials: true, responseType: 'text' }
    );
  }
}

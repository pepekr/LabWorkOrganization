import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable({ providedIn: 'root' })
export class AuthService {
  private _authorized$ = new BehaviorSubject<{ isLoggedIn: boolean, email?:string }>({isLoggedIn: false});

  constructor(private http: HttpClient) {
    this.checkAuth();
  }

  private checkAuth() {
    this.http.get<{ isLoggedIn: boolean, email:string }>(`${environment.backendUrl}/api/auth/isLoggedIn`)
      .subscribe({
        next: (data) => this._authorized$.next({isLoggedIn: data.isLoggedIn, email:data.email}),
        error: () => this._authorized$.next({isLoggedIn:false})
      });
  }

  get authorized$(): Observable<{ isLoggedIn: boolean, email?:string }> {
    return this._authorized$.asObservable();
  }

  refreshAuth() {
    this.checkAuth();
  }
}
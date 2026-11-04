import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, map, switchMap, tap } from "rxjs";
import { LoginResponseModel } from "../modals/login-response.model";

//oAuth imports
import { OAuthService } from 'angular-oauth2-oidc';
import { googleAuthConfig } from "../config/google-auth.config";

const LOGGED_IN_FLAG = 'is_logged_in';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:44321/api';

  private csrfToken: string | null = null;
  private currentUsername: string | null = null;
  private currentFullName: string | null = null;

  constructor(
    private http: HttpClient,
    private oAuth: OAuthService) {

    this.oAuth.configure(googleAuthConfig);
  }

  login(payload: any): Observable<LoginResponseModel> {
    const url = `${this.baseUrl}/Authorize/Login`;
    return this.http.post<LoginResponseModel>(url, payload, { withCredentials: true }).pipe(
      switchMap(res => this.establishSession(res))
    );
  }

  register(payload: any): Observable<any> {
    const url = `${this.baseUrl}/Authorize/Register`;
    return this.http.post<any>(url, payload);
  }

  refreshToken(): Observable<LoginResponseModel | null> {
    return this.http.post<LoginResponseModel>(`${this.baseUrl}/Authorize/Refresh`, {}, { withCredentials: true }).pipe(
      switchMap(res => this.establishSession(res))
    );
  }

  // The CSRF token embedded in the Login/Refresh response body is issued in the
  // same request that also sets the auth cookies, which unreliably pairs with
  // the cookie the browser actually ends up holding. A separate, dedicated
  // fetch after the session cookies are committed is what reliably works.
  private establishSession(res: LoginResponseModel): Observable<LoginResponseModel> {
    this.currentUsername = res.username;
    this.currentFullName = res.fullName;
    localStorage.setItem(LOGGED_IN_FLAG, 'true');

    return this.fetchCsrfToken().pipe(map(() => res));
  }

  fetchCsrfToken(): Observable<{ csrfToken: string }> {
    return this.http.get<{ csrfToken: string }>(`${this.baseUrl}/Authorize/CsrfToken`, { withCredentials: true }).pipe(
      tap(res => { this.csrfToken = res.csrfToken; })
    );
  }

  logout(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Authorize/Logout`, {}, { withCredentials: true }).pipe(
      tap(() => this.clearSession())
    );
  }

  clearSession(): void {
    this.csrfToken = null;
    this.currentUsername = null;
    this.currentFullName = null;
    localStorage.removeItem(LOGGED_IN_FLAG);
  }

  getCsrfToken(): string | null {
    return this.csrfToken;
  }

  googleLogin() {
    this.oAuth.loadDiscoveryDocument();
    this.oAuth.initLoginFlow();
  }

  async processGoogleLogin(): Promise<Observable<LoginResponseModel> | null> {
    await this.oAuth.loadDiscoveryDocumentAndTryLogin();
    if (!this.oAuth.hasValidIdToken()) {
        return null;
    }

    const idToken = this.oAuth.getIdToken();

    return this.http.post<LoginResponseModel>(`${this.baseUrl}/googleauth/login`, { idToken }, { withCredentials: true }).pipe(
      switchMap(res => this.establishSession(res))
    );
  }

  getCurrentUser() {
    if (!this.currentUsername) {
      return null;
    }

    return {
        username: this.currentUsername,
        fullName: this.currentFullName
    };
  }

  isAuthenticated(): boolean {
    return localStorage.getItem(LOGGED_IN_FLAG) === 'true';
  }
}

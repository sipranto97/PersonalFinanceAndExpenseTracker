import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

export interface ProfileUpdate {
  fullName?: string;
  profilePictureUrl?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private baseUrl = 'https://localhost:44321/api';

  private profileUpdated = new Subject<ProfileUpdate>();
  profileUpdated$ = this.profileUpdated.asObservable();

  constructor(private http: HttpClient) {}

  notifyProfileUpdated(change: ProfileUpdate): void {
    this.profileUpdated.next(change);
  }

  getAccount(): Observable<any>{
    const url = `${this.baseUrl}/Account`;
    return this.http.get<any>(url);
  }

  updateProfile(payload: any): Observable<any>{
    const url = `${this.baseUrl}/Account/Profile`;
    return this.http.put<any>(url, payload);
  }

  changePassword(payload: any): Observable<any>{
    const url = `${this.baseUrl}/Account/ChangePassword`;
    return this.http.post<any>(url, payload);
  }

  uploadProfilePicture(file: File): Observable<any>{
    const url = `${this.baseUrl}/Account/ProfilePicture`;
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(url, formData);
  }

  removeProfilePicture(): Observable<any>{
    const url = `${this.baseUrl}/Account/ProfilePicture`;
    return this.http.delete<any>(url);
  }
}

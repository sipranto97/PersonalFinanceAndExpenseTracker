import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CreateShareLinkPayload } from "../models/income-share.model";

@Injectable({
  providedIn: 'root'
})
export class IncomeShareService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createShareLink(payload: CreateShareLinkPayload): Observable<any>{
    const url = `${this.baseUrl}/IncomeShare`;
    return this.http.post<any>(url, payload);
  }

  getSharedIncomeHistory(token: string): Observable<any>{
    const url = `${this.baseUrl}/IncomeShare/${token}`;
    return this.http.get<any>(url);
  }
}

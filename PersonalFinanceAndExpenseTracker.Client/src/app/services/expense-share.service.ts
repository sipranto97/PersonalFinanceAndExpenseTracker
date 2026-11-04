import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CreateExpenseShareLinkPayload } from "../models/expense-share.model";

@Injectable({
  providedIn: 'root'
})
export class ExpenseShareService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createShareLink(payload: CreateExpenseShareLinkPayload): Observable<any>{
    const url = `${this.baseUrl}/ExpenseShare`;
    return this.http.post<any>(url, payload);
  }

  getSharedExpenseHistory(token: string): Observable<any>{
    const url = `${this.baseUrl}/ExpenseShare/${token}`;
    return this.http.get<any>(url);
  }
}

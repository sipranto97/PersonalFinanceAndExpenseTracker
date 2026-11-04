import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  getUserInfo(): Observable<any>{
    const url = `${this.baseUrl}/Dashboard/UserInfo`;
    return this.http.get<any>(url);
  }

  getBalance(startDate?: string, endDate?: string): Observable<any>{
    const url = `${this.baseUrl}/Dashboard/Balance`;
    return this.http.get<any>(url, { params: this.buildRangeParams(startDate, endDate) });
  }

  getOverviewChart(startDate: string, endDate: string): Observable<any>{
    const url = `${this.baseUrl}/Dashboard/OverviewChart`;
    return this.http.get<any>(url, { params: this.buildRangeParams(startDate, endDate) });
  }

  getExpenseChart(startDate: string, endDate: string): Observable<any>{
    const url = `${this.baseUrl}/Dashboard/ExpenseChart`;
    return this.http.get<any>(url, { params: this.buildRangeParams(startDate, endDate) });
  }

  getIncomeChart(startDate: string, endDate: string): Observable<any>{
    const url = `${this.baseUrl}/Dashboard/IncomeChart`;
    return this.http.get<any>(url, { params: this.buildRangeParams(startDate, endDate) });
  }

  private buildRangeParams(startDate?: string, endDate?: string): HttpParams {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    return params;
  }
}

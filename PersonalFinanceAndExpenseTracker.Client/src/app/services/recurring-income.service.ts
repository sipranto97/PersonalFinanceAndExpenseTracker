import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class RecurringIncomeService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createRecurringIncome(payload: any): Observable<any>{
    const url = `${this.baseUrl}/RecurringIncome`;
    return this.http.post<any>(url, payload);
  }

  getAllRecurringIncomes(): Observable<any>{
    const url = `${this.baseUrl}/RecurringIncome`;
    return this.http.get<any>(url);
  }

  updateRecurringIncome(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/RecurringIncome/${id}`;
    return this.http.put<any>(url, payload);
  }

  cancelRecurringIncome(id: number): Observable<any>{
    const url = `${this.baseUrl}/RecurringIncome/${id}/Cancel`;
    return this.http.post<any>(url, {});
  }
}

import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class RecurringExpenseService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createRecurringExpense(payload: any): Observable<any>{
    const url = `${this.baseUrl}/RecurringExpense`;
    return this.http.post<any>(url, payload);
  }

  getAllRecurringExpenses(): Observable<any>{
    const url = `${this.baseUrl}/RecurringExpense`;
    return this.http.get<any>(url);
  }

  updateRecurringExpense(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/RecurringExpense/${id}`;
    return this.http.put<any>(url, payload);
  }

  cancelRecurringExpense(id: number): Observable<any>{
    const url = `${this.baseUrl}/RecurringExpense/${id}/Cancel`;
    return this.http.post<any>(url, {});
  }
}

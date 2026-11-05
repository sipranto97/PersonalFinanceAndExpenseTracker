import { environment } from '../../environments/environment';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private baseUrl = environment.apiOrigin + '/api';

  constructor(private http: HttpClient) {}

  createExpense(payload: any): Observable<any>{
    const url = `${this.baseUrl}/Expense`;
    return this.http.post<any>(url, payload);
  }

  getAllExpense(): Observable<any>{
    const url = `${this.baseUrl}/Expense`;
    return this.http.get<any>(url);
  }

  getCategories(): Observable<any>{
    const url = `${this.baseUrl}/Expense/Categories`;
    return this.http.get<any>(url);
  }

  updateExpense(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/Expense/${id}`;
    return this.http.put<any>(url, payload);
  }

  deleteExpense(id: number): Observable<any>{
    const url = `${this.baseUrl}/Expense/${id}`;
    return this.http.delete<any>(url);
  }
}

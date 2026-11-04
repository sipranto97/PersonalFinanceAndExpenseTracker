import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class SavingsGoalService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createGoal(payload: any): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal`;
    return this.http.post<any>(url, payload);
  }

  getGoals(): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal`;
    return this.http.get<any>(url);
  }

  getActiveGoals(): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal/Active`;
    return this.http.get<any>(url);
  }

  updateGoal(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal/${id}`;
    return this.http.put<any>(url, payload);
  }

  deleteGoal(id: number): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal/${id}`;
    return this.http.delete<any>(url);
  }

  addContribution(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal/${id}/Contribute`;
    return this.http.post<any>(url, payload);
  }

  settleGoal(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/SavingsGoal/${id}/Settle`;
    return this.http.post<any>(url, payload);
  }
}

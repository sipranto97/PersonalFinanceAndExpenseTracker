import { environment } from '../../environments/environment';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Income } from "../models/income.model";

@Injectable({
  providedIn: 'root'
})
export class IncomeService {
  private baseUrl = environment.apiOrigin + '/api';

  constructor(private http: HttpClient) {}

  createIncome(payload: any): Observable<any>{
    const url = `${this.baseUrl}/Income`;
    return this.http.post<any>(url, payload);
  }

  getAllIncome(): Observable<any>{
    const url = `${this.baseUrl}/Income`;
    return this.http.get<any>(url);
  }

  getCategories(): Observable<any>{
    const url = `${this.baseUrl}/Income/Categories`;
    return this.http.get<any>(url);
  }

  updateIncome(id: number, payload: any): Observable<any>{
    const url = `${this.baseUrl}/Income/${id}`;
    return this.http.put<any>(url, payload);
  }

  deleteIncome(id: number): Observable<any>{
    const url = `${this.baseUrl}/Income/${id}`;
    return this.http.delete<any>(url);
  }
}
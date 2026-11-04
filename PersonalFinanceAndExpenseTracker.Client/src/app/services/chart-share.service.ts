import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CreateChartShareLinkPayload } from "../models/dashboard-chart.model";

@Injectable({
  providedIn: 'root'
})
export class ChartShareService {
  private baseUrl = 'https://localhost:44321/api';

  constructor(private http: HttpClient) {}

  createShareLink(payload: CreateChartShareLinkPayload): Observable<any>{
    const url = `${this.baseUrl}/ChartShare`;
    return this.http.post<any>(url, payload);
  }

  getSharedChartData(token: string): Observable<any>{
    const url = `${this.baseUrl}/ChartShare/${token}`;
    return this.http.get<any>(url);
  }
}

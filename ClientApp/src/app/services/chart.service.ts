import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Chart } from '../models/chart';

@Injectable()
export class ChartService {
  getChartDataUrl: string;
  getBoardDataUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) {
    this.getChartDataUrl = `${environment.baseUrl}/api/Chart/Data`;
    this.getBoardDataUrl = `${environment.baseUrl}/api/Board/Data`;
  }

  getChartData(): Observable<Chart[]> {
    const result = this.http.get<Chart[]>(this.getChartDataUrl);
    return result.pipe(map(response => {
      if (response == null) {
        catchError(this.errorHandler);
      }
      return response;
    }));
  }

  getBoardData(): Observable<Chart[]> {
    const result = this.http.get<Chart[]>(this.getBoardDataUrl);
    return result.pipe(map(response => {
      if (response == null) {
        catchError(this.errorHandler);
      }
      return response;
    }));
  }

  errorHandler(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.log(errorMessage);
    return throwError(errorMessage);
  }

}

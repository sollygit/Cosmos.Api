import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Candidate } from '../models/candidate';

@Injectable()
export class CandidateService {
  getAllUrl: string;
  getUrl: string;
  saveUrl: string;
  updateUrl: string;
  deleteUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };
  constructor(private http: HttpClient) {
      this.getAllUrl = `${environment.baseUrl}/api/Candidate/All`;
      this.getUrl = `${environment.baseUrl}/api/Candidate/Get`;
      this.saveUrl = `${environment.baseUrl}/api/Candidate/Create`;
      this.updateUrl = `${environment.baseUrl}/api/Candidate/Update`;
      this.deleteUrl = `${environment.baseUrl}/api/Candidate`;
  }

  getAll(): Observable<Candidate[]> {
    const result = this.http.get<Candidate[]>(this.getAllUrl);
    return result.pipe(map(response => {
      if (response == null) {
        catchError(this.errorHandler);
      }
      return response;
    }));
  }

  get(id: string): Observable<Candidate> {
      return this.http.get<Candidate>(`${this.getUrl}/${id}`)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  save(candidate: Candidate): Observable<Candidate> {
      return this.http.post<Candidate>(this.saveUrl, JSON.stringify(candidate), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  update(id: string, candidate: Candidate): Observable<Candidate> {
      return this.http.put<Candidate>(`${this.updateUrl}/${id}`, JSON.stringify(candidate), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  delete(id: string): Observable<Candidate> {
      return this.http.delete<Candidate>(`${this.deleteUrl}/${id}`)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
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

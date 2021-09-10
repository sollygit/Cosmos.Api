import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SessionStorageService } from 'src/app/services/sessionStorage.service';
import { CandidateSignalRService } from 'src/app/services/signal-r/candidate.signal-r.service';
import { CandidateService } from '../../services/candidate.service';
import { Candidate } from '../../models/candidate';
import { fadeInOut } from '../../services/animations';
import { DBkeys } from 'src/app/services/db-keys';

@Component({
  selector: 'app-candidates',
  templateUrl: './candidates.component.html',
  styleUrls: ['./candidates.component.css'],
  animations: [fadeInOut]
})
export class CandidatesComponent implements OnInit {
  candidateUrl = `${environment.baseUrl}/api/candidate`;
  active = true;

  constructor(
    private sessionStorage: SessionStorageService,
    private candidateService: CandidateService,
    private signalRService: CandidateSignalRService,
    private http: HttpClient) { }

  ngOnInit() {
    if (!this.signalRService.isConnected) {
      this.startConnection();
    }
  }

  startConnection() {
    this.signalRService.startConnection();
    this.signalRService.addSendCandidatesListener(this.isActive);
    this.startHttpRequest();
  }

  startHttpRequest = () => {
    this.http.get(this.candidateUrl)
      .subscribe(res => {
        console.log(res);
        this.getCandidates();
      });
  }

  getCandidates() {
    this.candidateService.getAll()
      .subscribe(candidates => {
        this.candidates = candidates.filter(c => c.isActive === this.isActive);
      });
  }

  delete(id: string) {
    const result = confirm('Are you sure?');
    if (result) {
      this.candidateService.delete(id).subscribe(() => {
        this.getCandidates();
      });
    }
  }

  set candidates(value: Candidate[]) {
    this.signalRService.data = value;
  }
  get candidates(): Candidate[] {
    return this.signalRService.data;
  }
  set isActive(value: boolean) {
    this.active = value;
    this.sessionStorage.set(DBkeys.IS_ACTIVE, value);
  }
  get isActive(): boolean {
    return this.sessionStorage.exists(DBkeys.IS_ACTIVE) ?
    this.sessionStorage.get(DBkeys.IS_ACTIVE) : this.active;
  }

}

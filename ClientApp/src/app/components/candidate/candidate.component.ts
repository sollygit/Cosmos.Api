import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { CandidateService } from '../../services/candidate.service';
import { Candidate } from '../../models/candidate';
import { fadeInOut } from '../../services/animations';
@Component({
  selector: 'app-candidate',
  templateUrl: './candidate.component.html',
  styleUrls: ['./candidate.component.css'],
  animations: [fadeInOut]
})
export class CandidateComponent implements OnInit {
  candidate: Candidate;
  id: string;

  constructor(private candidateService: CandidateService, private route: ActivatedRoute) {
    const id = 'id';
    if (this.route.snapshot.params[id]) {
      this.id = this.route.snapshot.params[id];
    }
  }

  ngOnInit() {
    this.loadCandidate();
  }

  loadCandidate() {
    this.candidateService.get(this.id)
      .subscribe(candidate => {
        this.candidate = candidate;
      });
  }

}

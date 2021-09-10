import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CandidateService } from '../../services/candidate.service';
import { Candidate } from '../../models/candidate';
import { Constants } from 'src/app/services/constants';
import { fadeInOut } from '../../services/animations';
import { mixinDisabled } from '@angular/material';
@Component({
  selector: 'app-candidate-add-edit',
  templateUrl: './candidate-add-edit.component.html',
  styleUrls: ['./candidate-add-edit.component.css'],
  animations: [fadeInOut]
})
export class CandidateAddEditComponent implements OnInit {
  form: FormGroup;
  actionType: string;
  id: string;
  errorMessage: any;
  existing: Candidate;
  technologyAll: string[] = Constants.TECHNOLOGIES;

  constructor(
    private candidateService: CandidateService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router) {

    const id = 'id';
    this.actionType = 'New';
    if (this.route.snapshot.params[id]) {
      this.id = this.route.snapshot.params[id];
    }
    this.form = this.formBuilder.group(
      {
        id: '',
        lastName: ['', [Validators.required]],
        firstName: ['', [Validators.required]],
        email: ['', [Validators.required]],
        technologies: [],
        isActive: [true]
      }
    );
  }

  ngOnInit() {
    if (this.id) {
      this.actionType = 'Edit';
      this.candidateService.get(this.id)
        .subscribe(data => (
          this.existing = data,
          this.form.controls['firstName'].setValue(data.firstName),
          this.form.controls['lastName'].setValue(data.lastName),
          this.form.controls['email'].setValue(data.email),
          this.form.controls['technologies'].setValue(data.technologies),
          this.form.controls['isActive'].setValue(data.isActive)
        ));
    }
  }

  save() {
    if (!this.form.valid) {
      return;
    }

    if (this.actionType === 'New') {
      const candidate: Candidate = {
        lastName: this.form.get('firstName').value,
        firstName: this.form.get('lastName').value,
        email: this.form.get('email').value,
        balance: 1000.00,
        points: 100,
        technologies: this.form.get('technologies').value || [],
        isActive: this.form.get('isActive').value
      };

      this.candidateService.save(candidate)
        .subscribe((data: Candidate) => {
          this.router.navigate(['/']);
        });
    }

    if (this.actionType === 'Edit') {
      const candidate: Candidate = {
        id: this.existing.id,
        registrationDate: this.existing.registrationDate,
        lastName: this.form.get('lastName').value,
        firstName: this.form.get('firstName').value,
        email: this.form.get('email').value,
        balance: this.existing.balance,
        points: this.existing.points,
        technologies: this.form.get('technologies').value,
        isActive: this.form.get('isActive').value
      };
      this.candidateService.update(candidate.id, candidate)
        .subscribe((data) => {
          this.router.navigate(['/']);
        });
    }
  }

  get lastName() { return this.form.get('lastName'); }
  get firstName() { return this.form.get('firstName'); }
  get email() { return this.form.get('email'); }
  get technologies() { return this.form.get('technologies'); }
  get isActive() { return this.form.get('isActive'); }
}

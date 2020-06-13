import { TestBed } from '@angular/core/testing';
import { CandidateService } from './candidate.service';
import { HttpClient } from '@angular/common/http';

describe('CandidateService', () => {
  let service: CandidateService;
  beforeEach(() => { service = new CandidateService(new HttpClient(null)); });

  it('#getAll should be defined', () => {
    expect(service.getAll()).toBeDefined();
  });
});

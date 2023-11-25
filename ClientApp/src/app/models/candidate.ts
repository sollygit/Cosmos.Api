import { DecimalPipe } from '@angular/common';

export class Candidate {
  id?: string;
  lastName: string;
  firstName: string;
  fullName?: string;
  email: string;
  balance: number;
  points: number;
  isActive?: true;
  technologies?: string[];
  registrationDate?: Date = new Date();
}

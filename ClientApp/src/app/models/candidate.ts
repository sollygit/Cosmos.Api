export class Candidate {
  id?: string;
  lastName: string;
  firstName: string;
  fullName?: string;
  email: string;
  isActive?: true;
  technologies?: string[];
  registrationDate?: Date = new Date();
}

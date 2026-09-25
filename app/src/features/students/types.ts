export interface Student {
  id: string;
  clientId: string;
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  createdAt: string;
}

export interface StudentSummary {
  id: string;
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  clientId: string;
  clientFullName: string;
  clientPhoneNumber: string;
}

export interface NewStudentRequest {
  fullName: string;
  birthDate: string | null;
  notes: string | null;
}

export interface SearchStudentsRequest {
  search: string;
  limit: number;
}

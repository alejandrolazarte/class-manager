import { StudentAppAccess } from "@/features/clients/types";

export interface Student {
  id: string;
  clientId: string;
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  email: string | null;
  createdAt: string;
  appAccess: StudentAppAccess;
  isOnTheTeam?: boolean;
  photoUrl: string | null;
}

export interface StudentSummary {
  id: string;
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  clientId: string;
  clientFullName: string;
  clientPhoneNumber: string;
  photoUrl: string | null;
}

export interface NewStudentRequest {
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  email: string | null;
  samePersonUserId?: string;
}

export interface UpdateStudentRequest {
  fullName: string;
  birthDate: string | null;
  notes: string | null;
  email: string | null;
}

export interface SearchStudentsRequest {
  search: string;
  limit: number;
}

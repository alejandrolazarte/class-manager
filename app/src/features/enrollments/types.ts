import { Weekday } from "@/features/classGroups/types";

export interface Enrollment {
  id: string;
  classGroupId: string;
  studentId: string;
  startDate: string;
  endDate: string | null;
}

export interface RosterEntry {
  enrollmentId: string;
  studentId: string;
  studentFullName: string;
  birthDate: string | null;
  clientId: string;
  clientFullName: string;
  startDate: string;
  endDate: string | null;
}

export interface StudentEnrollment {
  enrollmentId: string;
  classGroupId: string;
  classGroupName: string;
  weekdays: Weekday[];
  startTime: string;
  endTime: string;
  instructorFullName: string;
  startDate: string;
  endDate: string | null;
}

export interface EnrollStudentRequest {
  studentId: string;
  startDate?: string;
}

export interface EndEnrollmentRequest {
  endDate?: string;
}

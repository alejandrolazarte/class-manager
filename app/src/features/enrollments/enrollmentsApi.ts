import { httpClient } from "@/api/httpClient";
import {
  EndEnrollmentRequest,
  Enrollment,
  EnrollStudentRequest,
  RosterEntry,
  StudentEnrollment,
} from "@/features/enrollments/types";

const classGroupsPath = "/api/class-groups";
const studentsPath = "/api/students";
const enrollmentsPath = "/api/enrollments";
const enrollmentsSegment = "enrollments";
const endSegment = "end";

function classRosterPath(classGroupId: string): string {
  return `${classGroupsPath}/${encodeURIComponent(classGroupId)}/${enrollmentsSegment}`;
}

export function enrollStudent(
  classGroupId: string,
  request: EnrollStudentRequest,
): Promise<Enrollment> {
  return httpClient.post<Enrollment>(classRosterPath(classGroupId), request);
}

export function listClassRoster(classGroupId: string): Promise<RosterEntry[]> {
  return httpClient.get<RosterEntry[]>(classRosterPath(classGroupId));
}

export function listStudentEnrollments(studentId: string): Promise<StudentEnrollment[]> {
  return httpClient.get<StudentEnrollment[]>(
    `${studentsPath}/${encodeURIComponent(studentId)}/${enrollmentsSegment}`,
  );
}

export function endEnrollment(enrollmentId: string, request: EndEnrollmentRequest): Promise<void> {
  return httpClient.put<void>(
    `${enrollmentsPath}/${encodeURIComponent(enrollmentId)}/${endSegment}`,
    request,
  );
}

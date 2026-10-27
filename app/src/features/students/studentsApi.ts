import { httpClient } from "@/api/httpClient";
import { clientsPath } from "@/features/clients/clientsApi";
import {
  NewStudentRequest,
  SearchStudentsRequest,
  Student,
  StudentSummary,
  UpdateStudentRequest,
} from "@/features/students/types";

const studentsPath = "/api/students";
const studentsOfClientSegment = "students";

export function addStudent(clientId: string, request: NewStudentRequest): Promise<Student> {
  return httpClient.post<Student>(
    `${clientsPath}/${encodeURIComponent(clientId)}/${studentsOfClientSegment}`,
    request,
  );
}

export function searchStudents({
  search,
  limit,
}: SearchStudentsRequest): Promise<StudentSummary[]> {
  return httpClient.get<StudentSummary[]>(studentsPath, { search, limit });
}

export function updateStudent(studentId: string, request: UpdateStudentRequest): Promise<Student> {
  return httpClient.put<Student>(`${studentsPath}/${encodeURIComponent(studentId)}`, request);
}

export function deleteStudent(studentId: string): Promise<void> {
  return httpClient.delete<void>(`${studentsPath}/${encodeURIComponent(studentId)}`);
}

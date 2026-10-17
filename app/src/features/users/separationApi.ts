import { httpClient } from "@/api/httpClient";

export type SeparatedRole = "instructor" | "client" | "student";

export interface SeparatePersonRequest {
  fullName: string;
  email: string | null;
}

const separationPaths: Record<SeparatedRole, (id: string) => string> = {
  instructor: (instructorId) => `/api/instructors/${encodeURIComponent(instructorId)}/separation`,
  client: (clientId) => `/api/clients/${encodeURIComponent(clientId)}/separation`,
  student: (studentId) => `/api/students/${encodeURIComponent(studentId)}/separation`,
};

export function separatePerson(
  role: SeparatedRole,
  id: string,
  request: SeparatePersonRequest,
): Promise<void> {
  return httpClient.post<void>(separationPaths[role](id), request);
}

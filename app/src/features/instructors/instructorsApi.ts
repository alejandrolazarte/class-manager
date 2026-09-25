import { httpClient } from "@/api/httpClient";
import { Instructor, SaveInstructorRequest } from "@/features/instructors/types";

const instructorsPath = "/api/instructors";
const activeSegment = "active";
const includeInactiveParameters = { includeInactive: "true" };

function instructorPath(instructorId: string): string {
  return `${instructorsPath}/${encodeURIComponent(instructorId)}`;
}

export function listActiveInstructors(): Promise<Instructor[]> {
  return httpClient.get<Instructor[]>(instructorsPath);
}

export function listInstructorsIncludingInactive(): Promise<Instructor[]> {
  return httpClient.get<Instructor[]>(instructorsPath, includeInactiveParameters);
}

export function createInstructor(request: SaveInstructorRequest): Promise<Instructor> {
  return httpClient.post<Instructor>(instructorsPath, request);
}

export function renameInstructor(
  instructorId: string,
  request: SaveInstructorRequest,
): Promise<Instructor> {
  return httpClient.put<Instructor>(instructorPath(instructorId), request);
}

export function setInstructorActive(instructorId: string, isActive: boolean): Promise<Instructor> {
  return httpClient.put<Instructor>(`${instructorPath(instructorId)}/${activeSegment}`, {
    isActive,
  });
}

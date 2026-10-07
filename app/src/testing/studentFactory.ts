import { Student, StudentSummary } from "@/features/students/types";

export function buildStudent(overrides: Partial<Student> = {}): Student {
  return {
    id: "0192f0c5-0000-7000-8000-000000000001",
    clientId: "0192f0c4-0000-7000-8000-000000000001",
    fullName: "Tomás Pérez",
    birthDate: "2018-03-14",
    notes: null,
    email: null,
    createdAt: "2026-09-24T14:05:00Z",
    appAccess: { status: "NotInvited", invitedEmail: null },
    ...overrides,
  };
}

export function buildStudentSummary(overrides: Partial<StudentSummary> = {}): StudentSummary {
  return {
    id: "0192f0c5-0000-7000-8000-000000000001",
    fullName: "Tomás Pérez",
    birthDate: "2018-03-14",
    notes: null,
    clientId: "0192f0c4-0000-7000-8000-000000000001",
    clientFullName: "Ana Pérez",
    clientPhoneNumber: "+5491122334455",
    ...overrides,
  };
}

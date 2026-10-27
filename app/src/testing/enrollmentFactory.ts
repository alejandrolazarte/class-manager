import { RosterEntry, StudentEnrollment } from "@/features/enrollments/types";

export function buildRosterEntry(overrides: Partial<RosterEntry> = {}): RosterEntry {
  return {
    enrollmentId: "0192f0e1-0000-7000-8000-000000000001",
    studentId: "0192f0c5-0000-7000-8000-000000000001",
    studentFullName: "Tomás Pérez",
    birthDate: null,
    clientId: "0192f0c4-0000-7000-8000-000000000001",
    clientFullName: "Ana Pérez",
    startDate: "2026-09-24",
    endDate: null,
    ...overrides,
  };
}

export function buildStudentEnrollment(
  overrides: Partial<StudentEnrollment> = {},
): StudentEnrollment {
  return {
    enrollmentId: "0192f0e1-0000-7000-8000-000000000001",
    classGroupId: "0192f0d1-0000-7000-8000-000000000001",
    classGroupName: "Grupo de la mañana",
    weekdays: ["Monday", "Wednesday"],
    startTime: "18:00",
    endTime: "19:00",
    instructorFullName: "Laura Gómez",
    startDate: "2026-09-01",
    endDate: null,
    ...overrides,
  };
}

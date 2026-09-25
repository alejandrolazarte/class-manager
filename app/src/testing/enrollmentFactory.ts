import { RosterEntry } from "@/features/enrollments/types";

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

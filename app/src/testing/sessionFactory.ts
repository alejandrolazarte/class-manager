import { DaySession, SessionDetails, SessionStudent } from "@/features/sessions/types";

export const sessionDate = "2026-09-29";

export function buildSessionStudent(overrides: Partial<SessionStudent> = {}): SessionStudent {
  return {
    studentId: "0192f0c5-0000-7000-8000-000000000001",
    studentFullName: "Tomás Pérez",
    clientFullName: "Ana Pérez",
    birthDate: null,
    status: null,
    ...overrides,
  };
}

export function buildSessionDetails(overrides: Partial<SessionDetails> = {}): SessionDetails {
  return {
    classGroupId: "0192f0d2-0000-7000-8000-000000000001",
    classGroupName: "Natación inicial",
    date: sessionDate,
    startTime: "18:00",
    endTime: "18:45",
    originalStartTime: null,
    isCancelled: false,
    cancellationReason: null,
    canTakeAttendance: true,
    canReschedule: true,
    students: [buildSessionStudent()],
    ...overrides,
  };
}

export function buildDaySession(overrides: Partial<DaySession> = {}): DaySession {
  return {
    kind: "Group",
    classGroupId: "0192f0d2-0000-7000-8000-000000000001",
    privateLessonId: null,
    studentNames: [],
    isTrial: false,
    classGroupName: "Natación inicial",
    date: sessionDate,
    startTime: "18:00",
    endTime: "18:45",
    originalStartTime: null,
    instructorFullName: "Laura Gómez",
    location: null,
    isCancelled: false,
    cancellationReason: null,
    enrolledCount: 6,
    presentCount: 4,
    absentCount: 1,
    ...overrides,
  };
}

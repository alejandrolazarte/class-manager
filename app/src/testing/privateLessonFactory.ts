import { PrivateLesson, PrivateLessonStudent } from "@/features/privateLessons/types";
import { sessionDate } from "@/testing/sessionFactory";

export function buildPrivateLessonStudent(
  overrides: Partial<PrivateLessonStudent> = {},
): PrivateLessonStudent {
  return {
    studentId: "0192f0c5-0000-7000-8000-000000000001",
    studentFullName: "Tomás Pérez",
    birthDate: null,
    clientId: "0192f0c4-0000-7000-8000-000000000001",
    clientFullName: "Ana Pérez",
    status: null,
    ...overrides,
  };
}

export function buildPrivateLesson(overrides: Partial<PrivateLesson> = {}): PrivateLesson {
  return {
    id: "0192f0f1-0000-7000-8000-000000000001",
    instructorId: "0192f0b1-0000-7000-8000-000000000001",
    instructorFullName: "Laura Gómez",
    date: sessionDate,
    startTime: "10:00",
    endTime: "10:45",
    durationMinutes: 45,
    location: "Piscina Alboraya",
    notes: null,
    isCancelled: false,
    cancellationReason: null,
    seriesId: null,
    canTakeAttendance: true,
    students: [buildPrivateLessonStudent()],
    isTrial: false,
    trialPrice: null,
    ...overrides,
  };
}

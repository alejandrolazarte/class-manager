import { AttendanceStatus } from "@/features/sessions/types";

export interface PrivateLessonStudent {
  studentId: string;
  studentFullName: string;
  birthDate: string | null;
  clientId: string;
  clientFullName: string;
  status: AttendanceStatus | null;
}

export interface PrivateLesson {
  id: string;
  instructorId: string;
  instructorFullName: string;
  date: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  location: string | null;
  notes: string | null;
  isCancelled: boolean;
  cancellationReason: string | null;
  seriesId: string | null;
  canTakeAttendance: boolean;
  students: PrivateLessonStudent[];
  isTrial: boolean;
  trialPrice: number | null;
}

export interface PrivateLessonDetails {
  instructorId: string;
  date: string;
  startTime: string;
  durationMinutes: number;
  location: string | null;
  notes: string | null;
  isTrial: boolean;
  trialPrice: number | null;
}

export interface SchedulePrivateLessonRequest extends PrivateLessonDetails {
  studentIds: string[];
  repeatWeeks: number;
}

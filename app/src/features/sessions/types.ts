export type AttendanceStatus = "Present" | "Absent";

export interface DaySession {
  classGroupId: string;
  classGroupName: string;
  date: string;
  startTime: string;
  endTime: string;
  originalStartTime: string | null;
  instructorFullName: string;
  location: string | null;
  isCancelled: boolean;
  cancellationReason: string | null;
  enrolledCount: number;
  presentCount: number;
  absentCount: number;
}

export interface SessionStudent {
  studentId: string;
  studentFullName: string;
  clientFullName: string;
  birthDate: string | null;
  status: AttendanceStatus | null;
}

export interface SessionDetails {
  classGroupId: string;
  classGroupName: string;
  date: string;
  startTime: string;
  endTime: string;
  originalStartTime: string | null;
  isCancelled: boolean;
  cancellationReason: string | null;
  canTakeAttendance: boolean;
  canReschedule: boolean;
  students: SessionStudent[];
}

export interface CalendarDay {
  date: string;
  classCount: number;
  cancelledCount: number;
  pendingAttendanceCount: number;
}

export interface MonthCalendar {
  month: string;
  days: CalendarDay[];
}

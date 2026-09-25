export type AttendanceStatus = "Present" | "Absent";

export interface DaySession {
  classGroupId: string;
  classGroupName: string;
  date: string;
  startTime: string;
  endTime: string;
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
  isCancelled: boolean;
  cancellationReason: string | null;
  canTakeAttendance: boolean;
  students: SessionStudent[];
}

import { httpClient } from "@/api/httpClient";
import {
  AttendanceStatus,
  DaySession,
  MonthCalendar,
  SessionDetails,
} from "@/features/sessions/types";

const sessionsPath = "/api/sessions";
const calendarSegment = "calendar";
const classGroupsPath = "/api/class-groups";
const sessionsSegment = "sessions";
const attendanceSegment = "attendance";
const cancellationSegment = "cancellation";
const scheduleSegment = "schedule";
const substituteSegment = "substitute";

function sessionPath(classGroupId: string, sessionDate: string): string {
  return `${classGroupsPath}/${encodeURIComponent(classGroupId)}/${sessionsSegment}/${encodeURIComponent(sessionDate)}`;
}

export function listDaySessions(sessionDate: string): Promise<DaySession[]> {
  return httpClient.get<DaySession[]>(sessionsPath, { date: sessionDate });
}

export function getMonthCalendar(month: string): Promise<MonthCalendar> {
  return httpClient.get<MonthCalendar>(`${sessionsPath}/${calendarSegment}`, { month });
}

export function getSession(classGroupId: string, sessionDate: string): Promise<SessionDetails> {
  return httpClient.get<SessionDetails>(sessionPath(classGroupId, sessionDate));
}

export function recordAttendance(
  classGroupId: string,
  sessionDate: string,
  studentId: string,
  status: AttendanceStatus | null,
): Promise<void> {
  return httpClient.put<void>(
    `${sessionPath(classGroupId, sessionDate)}/${attendanceSegment}/${encodeURIComponent(studentId)}`,
    { status },
  );
}

export function cancelSession(
  classGroupId: string,
  sessionDate: string,
  reason: string | null,
): Promise<void> {
  return httpClient.put<void>(`${sessionPath(classGroupId, sessionDate)}/${cancellationSegment}`, {
    reason,
  });
}

export function restoreSession(classGroupId: string, sessionDate: string): Promise<void> {
  return httpClient.delete<void>(
    `${sessionPath(classGroupId, sessionDate)}/${cancellationSegment}`,
  );
}

export function rescheduleSession(
  classGroupId: string,
  sessionDate: string,
  startTime: string,
): Promise<void> {
  return httpClient.put<void>(`${sessionPath(classGroupId, sessionDate)}/${scheduleSegment}`, {
    startTime,
  });
}

export function restoreSessionSchedule(classGroupId: string, sessionDate: string): Promise<void> {
  return httpClient.delete<void>(`${sessionPath(classGroupId, sessionDate)}/${scheduleSegment}`);
}

export function assignSubstitute(
  classGroupId: string,
  sessionDate: string,
  instructorId: string,
): Promise<void> {
  return httpClient.put<void>(`${sessionPath(classGroupId, sessionDate)}/${substituteSegment}`, {
    instructorId,
  });
}

export function removeSubstitute(classGroupId: string, sessionDate: string): Promise<void> {
  return httpClient.delete<void>(`${sessionPath(classGroupId, sessionDate)}/${substituteSegment}`);
}

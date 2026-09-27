import { httpClient } from "@/api/httpClient";
import {
  PrivateLesson,
  PrivateLessonDetails,
  SchedulePrivateLessonRequest,
} from "@/features/privateLessons/types";
import { AttendanceStatus } from "@/features/sessions/types";

const privateLessonsPath = "/api/private-lessons";
const cancellationSegment = "cancellation";
const attendanceSegment = "attendance";

function privateLessonPath(privateLessonId: string): string {
  return `${privateLessonsPath}/${encodeURIComponent(privateLessonId)}`;
}

export function schedulePrivateLesson(
  request: SchedulePrivateLessonRequest,
): Promise<PrivateLesson[]> {
  return httpClient.post<PrivateLesson[]>(privateLessonsPath, request);
}

export function getPrivateLesson(privateLessonId: string): Promise<PrivateLesson> {
  return httpClient.get<PrivateLesson>(privateLessonPath(privateLessonId));
}

export function reschedulePrivateLesson(
  privateLessonId: string,
  details: PrivateLessonDetails,
): Promise<PrivateLesson> {
  return httpClient.put<PrivateLesson>(privateLessonPath(privateLessonId), details);
}

export function cancelPrivateLesson(
  privateLessonId: string,
  reason: string | null,
): Promise<PrivateLesson> {
  return httpClient.put<PrivateLesson>(
    `${privateLessonPath(privateLessonId)}/${cancellationSegment}`,
    { reason },
  );
}

export function restorePrivateLesson(privateLessonId: string): Promise<PrivateLesson> {
  return httpClient.delete<PrivateLesson>(
    `${privateLessonPath(privateLessonId)}/${cancellationSegment}`,
  );
}

export function recordPrivateLessonAttendance(
  privateLessonId: string,
  studentId: string,
  status: AttendanceStatus | null,
): Promise<void> {
  return httpClient.put<void>(
    `${privateLessonPath(privateLessonId)}/${attendanceSegment}/${encodeURIComponent(studentId)}`,
    { status },
  );
}

export function deletePrivateLesson(privateLessonId: string): Promise<void> {
  return httpClient.delete<void>(privateLessonPath(privateLessonId));
}

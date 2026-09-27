import { AttendanceStatus } from "@/features/sessions/types";

export const attendanceSwipeThreshold = 80;
export const attendanceSwipeLimit = 140;

export function attendanceStatusForSwipe(horizontalDistance: number): AttendanceStatus | null {
  if (horizontalDistance > attendanceSwipeThreshold) {
    return "Present";
  }
  if (horizontalDistance < -attendanceSwipeThreshold) {
    return "Absent";
  }
  return null;
}

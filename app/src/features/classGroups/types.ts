export type Weekday =
  "Monday" | "Tuesday" | "Wednesday" | "Thursday" | "Friday" | "Saturday" | "Sunday";

export interface ClassGroup {
  id: string;
  name: string;
  instructorId: string;
  instructorFullName: string;
  weekdays: Weekday[];
  startTime: string;
  endTime: string;
  durationMinutes: number;
  capacity: number;
  location: string | null;
  materialUrl: string | null;
  isActive: boolean;
  enrolledCount: number;
}

export interface SaveClassGroupRequest {
  name: string;
  instructorId: string;
  weekdays: Weekday[];
  startTime: string;
  durationMinutes: number;
  capacity: number;
  location: string | null;
  materialUrl: string | null;
}

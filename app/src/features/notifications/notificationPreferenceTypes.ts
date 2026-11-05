export type NotificationCategory =
  | "Orders"
  | "News"
  | "ClassChanges"
  | "InstructorComments"
  | "NewOrders"
  | "Attendance"
  | "StudentPlans"
  | "Invitations";

export type NotificationChannel = "Email" | "Push";

export type NotificationAudience = "Family" | "Team";

export interface NotificationChannelPreference {
  channel: NotificationChannel;
  isEnabled: boolean;
}

export interface NotificationPreference {
  category: NotificationCategory;
  audience: NotificationAudience;
  channels: NotificationChannelPreference[];
}

export interface UpdateNotificationPreferenceRequest {
  category: NotificationCategory;
  channel: NotificationChannel;
  isEnabled: boolean;
}

import {
  NotificationAudience,
  NotificationCategory,
  NotificationChannel,
  NotificationPreference,
} from "@/features/notifications/notificationPreferenceTypes";

export function buildNotificationPreference(
  category: NotificationCategory,
  audience: NotificationAudience,
  channels: NotificationChannel[] = ["Email", "Push"],
): NotificationPreference {
  return {
    category,
    audience,
    channels: channels.map((channel) => ({ channel, isEnabled: true })),
  };
}

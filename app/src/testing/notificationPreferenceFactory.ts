import {
  NotificationAudience,
  NotificationCategory,
  NotificationChannel,
  NotificationPreference,
  NotificationPreferences,
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

export const notificationPreferencesBusinessName = "Escuela Brazada";

export function buildNotificationPreferences(
  preferences: NotificationPreference[],
): NotificationPreferences {
  return { businessName: notificationPreferencesBusinessName, preferences };
}

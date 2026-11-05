import { httpClient } from "@/api/httpClient";
import {
  NotificationPreference,
  NotificationPreferences,
  UpdateNotificationPreferenceRequest,
} from "@/features/notifications/notificationPreferenceTypes";

const notificationPreferencesPath = "/api/me/notification-preferences";

export function getMyNotificationPreferences(): Promise<NotificationPreferences> {
  return httpClient.get<NotificationPreferences>(notificationPreferencesPath);
}

export function updateMyNotificationPreference(
  request: UpdateNotificationPreferenceRequest,
): Promise<NotificationPreference> {
  return httpClient.put<NotificationPreference>(notificationPreferencesPath, request);
}

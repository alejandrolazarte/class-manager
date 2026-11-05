import { httpClient } from "@/api/httpClient";
import {
  NotificationPreference,
  UpdateNotificationPreferenceRequest,
} from "@/features/notifications/notificationPreferenceTypes";

const notificationPreferencesPath = "/api/me/notification-preferences";

export function listMyNotificationPreferences(): Promise<NotificationPreference[]> {
  return httpClient.get<NotificationPreference[]>(notificationPreferencesPath);
}

export function updateMyNotificationPreference(
  request: UpdateNotificationPreferenceRequest,
): Promise<NotificationPreference> {
  return httpClient.put<NotificationPreference>(notificationPreferencesPath, request);
}

import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { useTeamPushNotifications } from "@/features/teamNotifications/useTeamNotifications";

export function TeamNotificationPreferencesScreen() {
  const notifications = useTeamPushNotifications();
  return <NotificationPreferencesScreen notifications={notifications} />;
}

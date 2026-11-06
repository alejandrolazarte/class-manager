import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { useTeamPushNotifications } from "@/features/teamNotifications/useTeamNotifications";
import { translate } from "@/i18n/translate";

export function TeamNotificationPreferencesScreen() {
  const notifications = useTeamPushNotifications();
  return (
    <NotificationPreferencesScreen
      notifications={notifications}
      deviceHint={translate("teamNotifications.hint")}
    />
  );
}

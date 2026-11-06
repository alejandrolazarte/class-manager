import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { useStudentAppNotifications } from "@/features/studentApp/push/useStudentAppNotifications";
import { translate } from "@/i18n/translate";

export function StudentAppNotificationPreferencesScreen() {
  const notifications = useStudentAppNotifications();
  return (
    <NotificationPreferencesScreen
      notifications={notifications}
      deviceHint={translate("student.notifications.hint")}
    />
  );
}

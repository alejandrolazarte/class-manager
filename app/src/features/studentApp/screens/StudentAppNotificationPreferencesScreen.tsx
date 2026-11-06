import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { useStudentAppNotifications } from "@/features/studentApp/push/useStudentAppNotifications";

export function StudentAppNotificationPreferencesScreen() {
  const notifications = useStudentAppNotifications();
  return <NotificationPreferencesScreen notifications={notifications} />;
}

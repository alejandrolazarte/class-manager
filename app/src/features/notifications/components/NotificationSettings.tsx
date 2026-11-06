import { View } from "react-native";
import { PushNotifications } from "@/features/notifications/usePushNotifications";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { SectionTitle } from "@/ui/SectionTitle";
import { ToggleSwitch } from "@/ui/ToggleSwitch";

interface NotificationSettingsProps {
  notifications: PushNotifications;
}

export function NotificationSettings({ notifications }: NotificationSettingsProps) {
  const { status, isOn, isPending, toggle } = notifications;
  if (status === "unavailable") {
    return null;
  }
  return (
    <View className="gap-1" testID="notification-settings">
      <SectionTitle title={translate("notificationPreferences.device")} isOverline />
      {status === "supported" ? (
        <ToggleSwitch
          label={translate("student.notifications.toggle")}
          value={isOn}
          onValueChange={(turnOn) => (isPending ? undefined : toggle(turnOn))}
        />
      ) : null}
      {status === "supported" ? null : (
        <AppText variant="caption" tone="muted">
          {translate(
            status === "blocked"
              ? "student.notifications.blocked"
              : "student.notifications.unsupported",
          )}
        </AppText>
      )}
    </View>
  );
}

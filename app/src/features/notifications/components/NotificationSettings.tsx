import { View } from "react-native";
import { PushNotifications } from "@/features/notifications/usePushNotifications";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ToggleSwitch } from "@/ui/ToggleSwitch";

interface NotificationSettingsProps {
  notifications: PushNotifications;
  hint: string;
}

export function NotificationSettings({ notifications, hint }: NotificationSettingsProps) {
  const { status, isOn, isPending, toggle } = notifications;
  if (status === "unavailable") {
    return null;
  }
  return (
    <View className="gap-3" testID="notification-settings">
      <AppText variant="headline" accessibilityRole="header">
        {translate("student.notifications.title")}
      </AppText>
      {status === "supported" ? (
        <ToggleSwitch
          label={translate("student.notifications.toggle")}
          value={isOn}
          onValueChange={(turnOn) => (isPending ? undefined : toggle(turnOn))}
        />
      ) : null}
      <AppText variant="caption" tone="muted">
        {status === "supported"
          ? hint
          : translate(
              status === "blocked"
                ? "student.notifications.blocked"
                : "student.notifications.unsupported",
            )}
      </AppText>
    </View>
  );
}

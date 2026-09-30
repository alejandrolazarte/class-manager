import { View } from "react-native";
import { PushNotifications } from "@/features/notifications/usePushNotifications";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
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
    <Card className="gap-2 p-4" testID="notification-settings">
      <View className="flex-row items-center gap-2">
        <Icon name="notifications" tone="primary" />
        <AppText variant="bodyStrong">{translate("family.notifications.title")}</AppText>
      </View>
      {status === "supported" ? (
        <ToggleSwitch
          label={translate("family.notifications.toggle")}
          value={isOn}
          onValueChange={(turnOn) => (isPending ? undefined : toggle(turnOn))}
        />
      ) : null}
      <AppText variant="caption" tone="muted">
        {status === "supported"
          ? hint
          : translate(
              status === "blocked"
                ? "family.notifications.blocked"
                : "family.notifications.unsupported",
            )}
      </AppText>
    </Card>
  );
}

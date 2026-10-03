import { NotificationSettings } from "@/features/notifications/components/NotificationSettings";
import { PushNotifications } from "@/features/notifications/usePushNotifications";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { translate, TranslationKey } from "@/i18n/translate";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";

export type DeviceSettingsSheetKind = "appearance" | "notifications";

const colorSchemeLabelKeys: Record<ColorSchemePreference, TranslationKey> = {
  system: "settings.appearance.system",
  light: "settings.appearance.light",
  dark: "settings.appearance.dark",
};

export function colorSchemeValue(preference: ColorSchemePreference): string {
  return translate(colorSchemeLabelKeys[preference]);
}

export function notificationsValue(notifications: PushNotifications): string {
  if (notifications.status === "supported") {
    return translate(notifications.isOn ? "settings.notificationsOn" : "settings.notificationsOff");
  }
  return translate(
    notifications.status === "blocked"
      ? "settings.notificationsBlocked"
      : "settings.notificationsUnsupported",
  );
}

interface DeviceSettingsSheetProps {
  kind: DeviceSettingsSheetKind | null;
  notifications: PushNotifications;
  notificationsHint: string;
  onClose: () => void;
}

export function DeviceSettingsSheet({
  kind,
  notifications,
  notificationsHint,
  onClose,
}: DeviceSettingsSheetProps) {
  if (kind === null) {
    return null;
  }
  return (
    <BottomSheet onClose={onClose}>
      {kind === "appearance" ? (
        <Card>
          <AppearanceSettings />
        </Card>
      ) : (
        <NotificationSettings notifications={notifications} hint={notificationsHint} />
      )}
      <Button label={translate("common.done")} onPress={onClose} />
    </BottomSheet>
  );
}

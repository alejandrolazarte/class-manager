import Constants from "expo-constants";
import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import {
  stopStudentAppNotifications,
  useStudentAppNotifications,
} from "@/features/studentApp/push/useStudentAppNotifications";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import {
  colorSchemeValue,
  DeviceSettingsSheet,
  DeviceSettingsSheetKind,
  notificationsValue,
} from "@/features/settings/components/DeviceSettingsSheet";
import { SettingsGroup, SettingsGroupList } from "@/features/settings/components/SettingsGroupList";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function StudentAppSettingsScreen() {
  const router = useRouter();
  const { signOut } = useSession();
  const { data: home } = useStudentAppHome();
  const { colorSchemePreference } = useTheme();
  const hasTeamAccount = useHasOtherAccountKind("student");
  const notifications = useStudentAppNotifications();
  const [openSheet, setOpenSheet] = useState<DeviceSettingsSheetKind | null>(null);
  const signOutOfThisDevice = async () => {
    await stopStudentAppNotifications().catch(() => undefined);
    await signOut();
  };

  const groups: SettingsGroup[] = [
    {
      titleKey: "settings.group.device",
      tone: "neutral",
      items: [
        {
          key: "appearance",
          isVisible: true,
          icon: "appearance",
          label: translate("settings.appearance"),
          value: colorSchemeValue(colorSchemePreference),
          onPress: () => setOpenSheet("appearance"),
        },
        {
          key: "notifications",
          isVisible: notifications.status !== "unavailable",
          icon: "notifications",
          label: translate("student.notifications.title"),
          value: notificationsValue(notifications),
          onPress: () => setOpenSheet("notifications"),
        },
      ],
    },
    {
      titleKey: "settings.group.account",
      tone: "neutral",
      items: [
        {
          key: "profile",
          isVisible: true,
          icon: "privateLesson",
          label: translate("profile.title"),
          onPress: () => router.push(routes.studentAppProfile),
        },
        {
          key: "accounts",
          isVisible: hasTeamAccount,
          icon: "business",
          label: translate("accounts.switch"),
          onPress: () => router.push(routes.chooseAccount),
        },
        {
          key: "signOut",
          isVisible: true,
          icon: "signOut",
          label: translate("settings.signOut"),
          onPress: signOutOfThisDevice,
          isDestructive: true,
        },
      ],
    },
  ];
  const appVersion = Constants.expoConfig?.version;

  return (
    <ScrollScreen header={<ScreenHeader title={translate("settings.title")} />}>
      {home === undefined ? null : (
        <Card className="flex-row items-center gap-3.5 p-4">
          <Avatar name={home.signedInFullName} />
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="heading">{home.signedInFullName}</AppText>
            <AppText variant="caption" tone="subtle">
              {home.businessName}
            </AppText>
          </View>
        </Card>
      )}
      <SettingsGroupList groups={groups} />
      {appVersion ? (
        <AppText variant="caption" tone="subtle" className="text-center">
          {translate("settings.version", { version: appVersion })}
        </AppText>
      ) : null}
      <DeviceSettingsSheet
        kind={openSheet}
        notifications={notifications}
        notificationsHint={translate("student.notifications.hint")}
        onClose={() => setOpenSheet(null)}
      />
    </ScrollScreen>
  );
}

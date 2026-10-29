import Constants from "expo-constants";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Linking, View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import {
  stopStudentAppNotifications,
  useStudentAppNotifications,
} from "@/features/studentApp/push/useStudentAppNotifications";
import { useStudentAppFamily } from "@/features/studentApp/useStudentAppFamily";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import {
  colorSchemeValue,
  DeviceSettingsSheet,
  DeviceSettingsSheetKind,
  notificationsValue,
} from "@/features/settings/components/DeviceSettingsSheet";
import { languageValue } from "@/features/settings/components/languageValue";
import { SettingsGroup, SettingsGroupList } from "@/features/settings/components/SettingsGroupList";
import { translate, translateCount } from "@/i18n/translate";
import { useLanguage } from "@/i18n/useLanguage";
import { routes } from "@/navigation/routes";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { useMyPhoto } from "@/features/userPhotos/useMyPhoto";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function StudentAppSettingsScreen() {
  const router = useRouter();
  const { signOut } = useSession();
  const { data: home } = useStudentAppHome();
  const myPhoto = useMyPhoto().data;
  const { data: family } = useStudentAppFamily();
  const { colorSchemePreference } = useTheme();
  const { language } = useLanguage();
  const hasTeamAccount = useHasOtherAccountKind("student");
  const notifications = useStudentAppNotifications();
  const [openSheet, setOpenSheet] = useState<DeviceSettingsSheetKind | null>(null);
  const signOutOfThisDevice = async () => {
    await stopStudentAppNotifications().catch(() => undefined);
    await signOut();
  };

  const location = home?.location ?? null;
  const locationMapsUrl = location?.mapsUrl ?? null;
  const groups: SettingsGroup[] = [
    {
      titleKey: "settings.group.location",
      items: [
        {
          key: "location",
          isVisible: location !== null,
          icon: "location",
          label: location?.name ?? "",
          value: locationMapsUrl === null ? undefined : translate("student.location.openMaps"),
          onPress: locationMapsUrl === null ? undefined : () => Linking.openURL(locationMapsUrl),
          isExternal: true,
        },
      ],
    },
    {
      titleKey: "settings.group.family",
      items: [
        {
          key: "family",
          isVisible: family !== undefined,
          icon: "students",
          label: translate("student.family.title"),
          value:
            family === undefined
              ? undefined
              : translateCount("student.family.count", family.members.length),
          onPress: () => router.push(routes.studentAppFamily),
        },
      ],
    },
    {
      titleKey: "settings.group.device",
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
          key: "language",
          isVisible: true,
          icon: "language",
          label: translate("settings.language"),
          value: languageValue(language),
          onPress: () => setOpenSheet("language"),
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
          key: "appFeedback",
          isVisible: true,
          icon: "comment",
          label: translate("appFeedback.title"),
          onPress: () => router.push(routes.studentAppFeedback),
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
        <View className="flex-row items-center gap-3 py-1">
          <Avatar name={home.signedInFullName} photoUrl={myPhoto?.url} />
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="heading">{home.signedInFullName}</AppText>
            <AppText variant="caption" tone="muted">
              {home.businessName}
            </AppText>
          </View>
        </View>
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
        colorsHint={translate("settings.colorsHint.student")}
        onClose={() => setOpenSheet(null)}
      />
    </ScrollScreen>
  );
}

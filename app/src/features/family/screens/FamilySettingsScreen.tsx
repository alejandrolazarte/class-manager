import { useRouter } from "expo-router";
import { View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import {
  stopFamilyNotifications,
  useFamilyNotifications,
} from "@/features/family/push/useFamilyNotifications";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { NotificationSettings } from "@/features/notifications/components/NotificationSettings";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function FamilySettingsScreen() {
  const router = useRouter();
  const { signOut } = useSession();
  const { data: home } = useFamilyHome();
  const hasTeamAccount = useHasOtherAccountKind("family");
  const notifications = useFamilyNotifications();
  return (
    <ScrollScreen header={<ScreenHeader title={translate("settings.title")} />}>
      {home === undefined ? null : (
        <Card className="flex-row items-center gap-3.5 p-4">
          <Avatar name={home.clientFullName} />
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong" className="text-base">
              {home.clientFullName}
            </AppText>
            <AppText variant="caption" tone="subtle">
              {home.businessName}
            </AppText>
          </View>
        </Card>
      )}
      {hasTeamAccount ? (
        <Card>
          <ListRow
            icon="business"
            label={translate("accounts.switch")}
            detail={translate("accounts.switchHint")}
            onPress={() => router.push(routes.chooseAccount)}
          />
        </Card>
      ) : null}
      <Card className="p-4">
        <NotificationSettings
          notifications={notifications}
          hint={translate("family.notifications.hint")}
        />
      </Card>
      <Card>
        <AppearanceSettings />
      </Card>
      <Button
        variant="dangerOutline"
        size="medium"
        icon="signOut"
        label={translate("settings.signOut")}
        onPress={async () => {
          await stopFamilyNotifications().catch(() => undefined);
          await signOut();
        }}
      />
    </ScrollScreen>
  );
}

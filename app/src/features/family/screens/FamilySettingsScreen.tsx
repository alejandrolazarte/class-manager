import { View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { NotificationSettings } from "@/features/family/components/NotificationSettings";
import { stopFamilyNotifications } from "@/features/family/push/useFamilyNotifications";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function FamilySettingsScreen() {
  const { signOut } = useSession();
  const { data: home } = useFamilyHome();
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
      <Card className="p-4">
        <NotificationSettings />
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

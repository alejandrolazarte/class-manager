import { useRouter } from "expo-router";
import { Pressable, ScrollView, View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Button } from "@/ui/Button";
import { Icon } from "@/ui/Icon";
import { AppText } from "@/ui/AppText";

interface SettingsRowProps {
  label: string;
  detail?: string;
  onPress: () => void;
}

function SettingsRow({ label, detail, onPress }: SettingsRowProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-4"
    >
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{label}</AppText>
        {detail ? (
          <AppText variant="caption" tone="muted">
            {detail}
          </AppText>
        ) : null}
      </View>
      <Icon name="next" size="medium" tone="disabled-foreground" />
    </Pressable>
  );
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  return (
    <ScrollView className="flex-1 bg-background" contentContainerClassName="gap-6 pb-12 pt-4">
      <View>
        <SettingsRow
          label={translate("settings.business")}
          detail={business.name}
          onPress={() => router.push(routes.businessSettings)}
        />
        <SettingsRow
          label={translate("settings.monthlyFee")}
          detail={
            business.defaultMonthlyFee === null
              ? translate("settings.monthlyFeeMissing")
              : formatMoney(business.defaultMonthlyFee, business.currencyCode)
          }
          onPress={() => router.push(routes.defaultMonthlyFee)}
        />
        <SettingsRow
          label={translate("settings.classPacks")}
          detail={translate("settings.classPacksHint")}
          onPress={() => router.push(routes.classPacks)}
        />
        <SettingsRow
          label={translate("settings.instructors")}
          onPress={() => router.push(routes.instructors)}
        />
      </View>
      <View className="px-4">
        <Button variant="secondary" label={translate("settings.signOut")} onPress={signOut} />
      </View>
    </ScrollView>
  );
}

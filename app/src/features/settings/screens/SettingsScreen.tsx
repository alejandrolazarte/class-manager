import { useRouter } from "expo-router";
import { View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  return (
    <ScrollScreen header={<ScreenHeader title={translate("settings.title")} />}>
      <Card
        onPress={() => router.push(routes.businessSettings)}
        accessibilityLabel={translate("settings.business")}
        className="flex-row items-center gap-3.5 p-4"
      >
        <BrandMark size="small" />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong" className="text-base">
            {business.name}
          </AppText>
          <AppText variant="caption" tone="subtle">
            {translate("settings.businessSummary", { currency: business.currencyCode })}
          </AppText>
        </View>
      </Card>
      <Card>
        <ListRow
          icon="monthlyFee"
          label={translate("settings.monthlyFee")}
          detail={
            business.defaultMonthlyFee === null
              ? translate("settings.monthlyFeeMissing")
              : formatMoney(business.defaultMonthlyFee, business.currencyCode)
          }
          onPress={() => router.push(routes.defaultMonthlyFee)}
        />
        <ListDivider />
        <ListRow
          icon="instructors"
          label={translate("settings.instructors")}
          onPress={() => router.push(routes.instructors)}
        />
        <ListDivider />
        <ListRow
          icon="classPacks"
          label={translate("settings.classPacks")}
          detail={translate("settings.classPacksHint")}
          onPress={() => router.push(routes.classPacks)}
        />
        <ListDivider />
        <ListRow
          icon="importExport"
          label={translate("settings.importExport")}
          detail={translate("settings.importExportHint")}
          onPress={() => router.push(routes.importExport)}
        />
        <ListDivider />
        <ListRow
          icon="business"
          label={translate("settings.business")}
          detail={business.name}
          onPress={() => router.push(routes.businessSettings)}
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
        onPress={signOut}
      />
    </ScrollScreen>
  );
}

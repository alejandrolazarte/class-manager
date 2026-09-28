import { useRouter } from "expo-router";
import { Fragment, ReactElement } from "react";
import { View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
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

interface SettingsRow {
  key: string;
  isVisible: boolean;
  row: ReactElement;
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  const canManageBusiness = useCan(permissions.businessManage);
  const canViewInstructors = useCan(permissions.instructorsView);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canImportExport = useCan(permissions.importExportRun);
  const canViewTeam = useCan(permissions.membersView);
  const rows: SettingsRow[] = [
    {
      key: "team",
      isVisible: canViewTeam,
      row: (
        <ListRow
          icon="students"
          label={translate("settings.team")}
          detail={translate("settings.teamHint")}
          onPress={() => router.push(routes.team)}
        />
      ),
    },
    {
      key: "monthlyFee",
      isVisible: canManageBusiness,
      row: (
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
      ),
    },
    {
      key: "instructors",
      isVisible: canViewInstructors,
      row: (
        <ListRow
          icon="instructors"
          label={translate("settings.instructors")}
          onPress={() => router.push(routes.instructors)}
        />
      ),
    },
    {
      key: "classPacks",
      isVisible: canViewClassPacks,
      row: (
        <ListRow
          icon="classPacks"
          label={translate("settings.classPacks")}
          detail={translate("settings.classPacksHint")}
          onPress={() => router.push(routes.classPacks)}
        />
      ),
    },
    {
      key: "importExport",
      isVisible: canImportExport,
      row: (
        <ListRow
          icon="importExport"
          label={translate("settings.importExport")}
          detail={translate("settings.importExportHint")}
          onPress={() => router.push(routes.importExport)}
        />
      ),
    },
    {
      key: "business",
      isVisible: canManageBusiness,
      row: (
        <ListRow
          icon="business"
          label={translate("settings.business")}
          detail={business.name}
          onPress={() => router.push(routes.businessSettings)}
        />
      ),
    },
  ];
  const visibleRows = rows.filter((settingsRow) => settingsRow.isVisible);
  return (
    <ScrollScreen header={<ScreenHeader title={translate("settings.title")} />}>
      <Card
        onPress={canManageBusiness ? () => router.push(routes.businessSettings) : undefined}
        accessibilityLabel={canManageBusiness ? translate("settings.business") : undefined}
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
      {visibleRows.length > 0 ? (
        <Card>
          {visibleRows.map((settingsRow, index) => (
            <Fragment key={settingsRow.key}>
              {index > 0 ? <ListDivider /> : null}
              {settingsRow.row}
            </Fragment>
          ))}
        </Card>
      ) : null}
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

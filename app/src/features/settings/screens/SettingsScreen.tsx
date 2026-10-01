import { useRouter } from "expo-router";
import { Fragment, ReactElement } from "react";
import { View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import { useBranches } from "@/features/branches/useBranches";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { NotificationSettings } from "@/features/notifications/components/NotificationSettings";
import {
  stopTeamPushNotifications,
  useTeamPushNotifications,
} from "@/features/teamNotifications/useTeamNotifications";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";

interface SettingsRow {
  key: string;
  isVisible: boolean;
  row: ReactElement;
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  const pushNotifications = useTeamPushNotifications();
  const canManageBusiness = useCan(permissions.businessManage);
  const canViewInstructors = useCan(permissions.instructorsView);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canViewProducts = useCan(permissions.productsView);
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const canImportExport = useCan(permissions.importExportRun);
  const canManageAnnouncements = useCan(permissions.announcementsManage);
  const canViewTeam = useCan(permissions.membersView);
  const canCreateBranches = useCan(permissions.branchesCreate);
  const { data: branches = [] } = useBranches();
  const hasFamilyAccount = useHasOtherAccountKind("team");
  const currentBranchName = branches.find((branch) => branch.isCurrent)?.name ?? business.name;
  const currentBrand = useCurrentBrand();
  const rows: SettingsRow[] = [
    {
      key: "brand",
      isVisible: canManageBusiness,
      row: (
        <ListRow
          icon="palette"
          label={translate("settings.brand")}
          detail={translate("settings.brandHint")}
          onPress={() => router.push(routes.brandSettings)}
        />
      ),
    },
    {
      key: "accounts",
      isVisible: hasFamilyAccount,
      row: (
        <ListRow
          icon="home"
          label={translate("accounts.switch")}
          detail={translate("accounts.switchHint")}
          onPress={() => router.push(routes.chooseAccount)}
        />
      ),
    },
    {
      key: "branches",
      isVisible: canCreateBranches || branches.length > 1,
      row: (
        <ListRow
          icon="business"
          label={translate("settings.branches")}
          detail={branches.length > 1 ? currentBranchName : translate("settings.branchesHint")}
          onPress={() => router.push(routes.branches)}
        />
      ),
    },
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
      key: "roles",
      isVisible: canViewTeam,
      row: (
        <ListRow
          icon="roles"
          label={translate("settings.roles")}
          detail={translate("settings.rolesHint")}
          onPress={() => router.push(routes.roles)}
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
      key: "products",
      isVisible: canViewProducts,
      row: (
        <ListRow
          icon="products"
          label={translate("settings.products")}
          detail={translate("settings.productsHint")}
          onPress={() => router.push(routes.products)}
        />
      ),
    },
    {
      key: "orders",
      isVisible: canViewOrders,
      row: (
        <ListRow
          icon="orders"
          label={translate("settings.orders")}
          detail={translate("settings.ordersHint")}
          onPress={() => router.push(routes.orders("settings"))}
        />
      ),
    },
    {
      key: "achievements",
      isVisible: canManageBusiness,
      row: (
        <ListRow
          icon="achievements"
          label={translate("settings.achievements")}
          detail={translate("settings.achievementsHint")}
          onPress={() => router.push(routes.achievementSettings)}
        />
      ),
    },
    {
      key: "announcements",
      isVisible: canManageAnnouncements,
      row: (
        <ListRow
          icon="announcement"
          label={translate("settings.announcements")}
          detail={translate("settings.announcementsHint")}
          onPress={() => router.push(routes.announcements)}
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
  ];
  const visibleRows = rows.filter((settingsRow) => settingsRow.isVisible);
  return (
    <ScrollScreen header={<ScreenHeader title={translate("settings.title")} />}>
      <Card
        onPress={canManageBusiness ? () => router.push(routes.businessSettings) : undefined}
        accessibilityLabel={canManageBusiness ? translate("settings.business") : undefined}
        className="flex-row items-center gap-3.5 p-4"
      >
        {currentBrand?.brand ? <CurrentBrandLogo /> : <BrandMark size="small" />}
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong" className="text-base">
            {business.name}
          </AppText>
          <AppText variant="caption" tone="subtle">
            {translate("settings.businessSummary", { currency: business.currencyCode })}
          </AppText>
        </View>
        {canManageBusiness ? <Icon name="next" tone="subtle-foreground" /> : null}
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
      <NotificationSettings
        notifications={pushNotifications}
        hint={translate("teamNotifications.hint")}
      />
      <Card>
        <AppearanceSettings />
      </Card>
      <Button
        variant="dangerOutline"
        size="medium"
        icon="signOut"
        label={translate("settings.signOut")}
        onPress={async () => {
          await stopTeamPushNotifications().catch(() => undefined);
          await signOut();
        }}
      />
    </ScrollScreen>
  );
}

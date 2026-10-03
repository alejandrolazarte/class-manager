import Constants from "expo-constants";
import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { useBranches } from "@/features/branches/useBranches";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { formatMoney } from "@/features/fees/money";
import { useCan, useRoleCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { useProducts } from "@/features/products/useProducts";
import {
  colorSchemeValue,
  DeviceSettingsSheet,
  DeviceSettingsSheetKind,
  notificationsValue,
} from "@/features/settings/components/DeviceSettingsSheet";
import { SettingsGroup, SettingsGroupList } from "@/features/settings/components/SettingsGroupList";
import {
  stopTeamPushNotifications,
  useTeamPushNotifications,
} from "@/features/teamNotifications/useTeamNotifications";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { useSubscription } from "@/features/subscriptions/useSubscription";

function countValue(count: number | undefined): string | undefined {
  return count === undefined || count === 0 ? undefined : String(count);
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  const { colorSchemePreference } = useTheme();
  const pushNotifications = useTeamPushNotifications();
  const [openSheet, setOpenSheet] = useState<DeviceSettingsSheetKind | null>(null);
  const canManageBusiness = useCan(permissions.businessManage);
  const canViewInstructors = useCan(permissions.instructorsView);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canViewProducts = useCan(permissions.productsView);
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  const canImportExport = useRoleCan(permissions.importExportRun);
  const canManageAnnouncements = useCan(permissions.announcementsManage);
  const canViewTeam = useCan(permissions.membersView);
  const canCreateBranches = useCan(permissions.branchesCreate);
  const subscription = useSubscription();
  const { data: branches = [] } = useBranches();
  const { data: classPacks } = useClassPacks(false, { enabled: canViewClassPacks });
  const { data: products } = useProducts(false, { enabled: canViewProducts });
  const hasFamilyAccount = useHasOtherAccountKind("team");
  const currentBranchName = branches.find((branch) => branch.isCurrent)?.name ?? business.name;
  const currentBrand = useCurrentBrand();
  const hasCustomBrand =
    currentBrand?.brand?.themeColor != null || currentBrand?.brand?.logoUpdatedAt != null;
  const signOutOfThisDevice = async () => {
    await stopTeamPushNotifications().catch(() => undefined);
    await signOut();
  };

  const groups: SettingsGroup[] = [
    {
      titleKey: "settings.group.business",
      tone: "primary",
      items: [
        {
          key: "brand",
          isVisible: canManageBusiness,
          icon: "palette",
          label: translate("settings.brand"),
          value: hasCustomBrand ? translate("settings.brandCustom") : undefined,
          onPress: () => router.push(routes.brandSettings),
        },
        {
          key: "plan",
          isVisible: true,
          icon: "plan",
          label: translate("settings.plan"),
          value: planLabel(subscription.planCode),
          onPress: () => router.push(routes.plan("settings")),
        },
        {
          key: "branches",
          isVisible: canCreateBranches || branches.length > 1,
          icon: "business",
          label: translate("settings.branches"),
          value: branches.length > 1 ? currentBranchName : countValue(branches.length),
          onPress: () => router.push(routes.branches),
        },
        {
          key: "monthlyFee",
          isVisible: canManageBusiness,
          icon: "monthlyFee",
          label: translate("settings.monthlyFee"),
          value:
            business.defaultMonthlyFee === null
              ? translate("settings.monthlyFeeMissing")
              : formatMoney(business.defaultMonthlyFee, business.currencyCode),
          onPress: () => router.push(routes.defaultMonthlyFee),
        },
      ],
    },
    {
      titleKey: "settings.group.people",
      tone: "accent",
      items: [
        {
          key: "team",
          isVisible: canViewTeam,
          icon: "students",
          label: translate("settings.team"),
          onPress: () => router.push(routes.team),
        },
        {
          key: "roles",
          isVisible: canViewTeam,
          icon: "roles",
          label: translate("settings.roles"),
          onPress: () => router.push(routes.roles),
        },
        {
          key: "instructors",
          isVisible: canViewInstructors,
          icon: "instructors",
          label: translate("settings.instructors"),
          onPress: () => router.push(routes.instructors),
        },
        {
          key: "achievements",
          isVisible: canManageBusiness,
          icon: "achievements",
          label: translate("settings.achievements"),
          onPress: () => router.push(routes.achievementSettings),
        },
        {
          key: "announcements",
          isVisible: canManageAnnouncements,
          icon: "announcement",
          label: translate("settings.announcements"),
          onPress: () => router.push(routes.announcements),
        },
      ],
    },
    {
      titleKey: "settings.group.shop",
      tone: "primary",
      items: [
        {
          key: "classPacks",
          isVisible: canViewClassPacks,
          icon: "classPacks",
          label: translate("settings.classPacks"),
          value: countValue(classPacks?.length),
          onPress: () => router.push(routes.classPacks),
        },
        {
          key: "products",
          isVisible: canViewProducts,
          icon: "products",
          label: translate("settings.products"),
          value: countValue(products?.length),
          onPress: () => router.push(routes.products),
        },
        {
          key: "orders",
          isVisible: canViewOrders && !canViewPayments,
          icon: "orders",
          label: translate("settings.orders"),
          onPress: () => router.push(routes.orders("settings")),
        },
      ],
    },
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
          isVisible: pushNotifications.status !== "unavailable",
          icon: "notifications",
          label: translate("family.notifications.title"),
          value: notificationsValue(pushNotifications),
          onPress: () => setOpenSheet("notifications"),
        },
        {
          key: "importExport",
          isVisible: canImportExport,
          icon: "importExport",
          label: translate("settings.importExport"),
          onPress: () => router.push(routes.importExport),
        },
      ],
    },
    {
      titleKey: "settings.group.account",
      tone: "neutral",
      items: [
        {
          key: "accounts",
          isVisible: hasFamilyAccount,
          icon: "home",
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
      <Card
        onPress={canManageBusiness ? () => router.push(routes.businessSettings) : undefined}
        accessibilityLabel={canManageBusiness ? translate("settings.business") : undefined}
        className="flex-row items-center gap-3.5 p-4"
      >
        {currentBrand?.brand ? <CurrentBrandLogo /> : <BrandMark size="small" />}
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="heading">{business.name}</AppText>
          <AppText variant="caption" tone="subtle">
            {translate("settings.businessSummary", { currency: business.currencyCode })}
          </AppText>
        </View>
        {canManageBusiness ? <Icon name="next" tone="subtle-foreground" /> : null}
      </Card>
      <SettingsGroupList groups={groups} />
      {appVersion ? (
        <AppText variant="caption" tone="subtle" className="text-center">
          {translate("settings.version", { version: appVersion })}
        </AppText>
      ) : null}
      <DeviceSettingsSheet
        kind={openSheet}
        notifications={pushNotifications}
        notificationsHint={translate("teamNotifications.hint")}
        onClose={() => setOpenSheet(null)}
      />
    </ScrollScreen>
  );
}

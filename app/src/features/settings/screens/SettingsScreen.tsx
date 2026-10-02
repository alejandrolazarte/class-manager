import Constants from "expo-constants";
import { useRouter } from "expo-router";
import { Fragment, useState } from "react";
import { View } from "react-native";
import { useHasOtherAccountKind } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { useBranches } from "@/features/branches/useBranches";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { NotificationSettings } from "@/features/notifications/components/NotificationSettings";
import { PushNotifications } from "@/features/notifications/usePushNotifications";
import { useProducts } from "@/features/products/useProducts";
import { SettingsRow, SettingsRowTone } from "@/features/settings/components/SettingsRow";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import {
  stopTeamPushNotifications,
  useTeamPushNotifications,
} from "@/features/teamNotifications/useTeamNotifications";
import { translate, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon, IconName } from "@/ui/Icon";
import { ListDivider } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";

interface SettingsItem {
  key: string;
  isVisible: boolean;
  icon: IconName;
  label: string;
  value?: string;
  onPress: () => void;
  isDestructive?: boolean;
}

const destructiveTone: SettingsRowTone = "danger";

interface SettingsGroup {
  titleKey: TranslationKey;
  tone: SettingsRowTone;
  items: SettingsItem[];
}

type OpenSheet = "appearance" | "notifications" | null;

const colorSchemeLabelKeys: Record<ColorSchemePreference, TranslationKey> = {
  system: "settings.appearance.system",
  light: "settings.appearance.light",
  dark: "settings.appearance.dark",
};

function notificationsValue(notifications: PushNotifications): string {
  if (notifications.status === "supported") {
    return translate(notifications.isOn ? "settings.notificationsOn" : "settings.notificationsOff");
  }
  return translate(
    notifications.status === "blocked"
      ? "settings.notificationsBlocked"
      : "settings.notificationsUnsupported",
  );
}

function countValue(count: number | undefined): string | undefined {
  return count === undefined || count === 0 ? undefined : String(count);
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  const { colorSchemePreference } = useTheme();
  const pushNotifications = useTeamPushNotifications();
  const [openSheet, setOpenSheet] = useState<OpenSheet>(null);
  const canManageBusiness = useCan(permissions.businessManage);
  const canViewInstructors = useCan(permissions.instructorsView);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canViewProducts = useCan(permissions.productsView);
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  const canImportExport = useCan(permissions.importExportRun);
  const canManageAnnouncements = useCan(permissions.announcementsManage);
  const canViewTeam = useCan(permissions.membersView);
  const canCreateBranches = useCan(permissions.branchesCreate);
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
          value: translate(colorSchemeLabelKeys[colorSchemePreference]),
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
  const visibleGroups = groups
    .map((group) => ({ ...group, items: group.items.filter((item) => item.isVisible) }))
    .filter((group) => group.items.length > 0);
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
          <AppText variant="bodyStrong" className="text-base">
            {business.name}
          </AppText>
          <AppText variant="caption" tone="subtle">
            {translate("settings.businessSummary", { currency: business.currencyCode })}
          </AppText>
        </View>
        {canManageBusiness ? <Icon name="next" tone="subtle-foreground" /> : null}
      </Card>
      {visibleGroups.map((group) => (
        <View key={group.titleKey} className="gap-1.5">
          <View className="px-1">
            <SectionTitle title={translate(group.titleKey)} isOverline />
          </View>
          <Card>
            {group.items.map((item, index) => (
              <Fragment key={item.key}>
                {index > 0 ? <ListDivider /> : null}
                <SettingsRow
                  icon={item.icon}
                  tone={item.isDestructive ? destructiveTone : group.tone}
                  label={item.label}
                  value={item.value}
                  onPress={item.onPress}
                  isDestructive={item.isDestructive}
                />
              </Fragment>
            ))}
          </Card>
        </View>
      ))}
      {appVersion ? (
        <AppText variant="caption" tone="subtle" className="text-center">
          {translate("settings.version", { version: appVersion })}
        </AppText>
      ) : null}
      {openSheet === "appearance" ? (
        <BottomSheet onClose={() => setOpenSheet(null)}>
          <Card>
            <AppearanceSettings />
          </Card>
          <Button label={translate("common.done")} onPress={() => setOpenSheet(null)} />
        </BottomSheet>
      ) : null}
      {openSheet === "notifications" ? (
        <BottomSheet onClose={() => setOpenSheet(null)}>
          <NotificationSettings
            notifications={pushNotifications}
            hint={translate("teamNotifications.hint")}
          />
          <Button label={translate("common.done")} onPress={() => setOpenSheet(null)} />
        </BottomSheet>
      ) : null}
    </ScrollScreen>
  );
}

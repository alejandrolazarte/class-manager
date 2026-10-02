import { Tabs } from "expo-router";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { MemberProvider } from "@/features/members/MemberProvider";
import { permissions as memberPermissions } from "@/features/members/permissions";
import { usePendingOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { TabIcon, useTabBarScreenOptions } from "@/navigation/tabBar";
import { isTabVisible, tabDefinitions } from "@/navigation/tabDefinitions";

function MemberTabs() {
  const screenOptions = useTabBarScreenOptions();
  const { permissions } = useCurrentMember();
  const canViewOrders = [memberPermissions.ordersViewAll, memberPermissions.ordersViewOwn].some(
    (permission) => permissions.includes(permission),
  );
  const pendingOrders = usePendingOrders({ enabled: canViewOrders });
  return (
    <Tabs screenOptions={screenOptions}>
      {tabDefinitions.map((tab) => (
        <Tabs.Screen
          key={tab.name}
          name={tab.name}
          options={{
            title: translate(tab.titleKey),
            href: isTabVisible(tab, permissions) ? undefined : null,
            tabBarBadge:
              tab.showsPendingOrders && pendingOrders.count > 0 ? pendingOrders.count : undefined,
            tabBarIcon: ({ focused }) => <TabIcon icon={tab.icon} isFocused={focused} />,
          }}
        />
      ))}
    </Tabs>
  );
}

export default function TabsLayout() {
  return (
    <RequireSessionKind kind="team">
      <BusinessProvider>
        <MemberProvider>
          <BrandProvider audience="team">
            <MemberTabs />
          </BrandProvider>
        </MemberProvider>
      </BusinessProvider>
    </RequireSessionKind>
  );
}

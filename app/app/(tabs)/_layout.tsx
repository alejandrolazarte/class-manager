import { Tabs } from "expo-router";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { MemberProvider } from "@/features/members/MemberProvider";
import { translate } from "@/i18n/translate";
import { TabIcon, useTabBarScreenOptions } from "@/navigation/tabBar";
import { isTabVisible, tabDefinitions } from "@/navigation/tabDefinitions";

function MemberTabs() {
  const screenOptions = useTabBarScreenOptions();
  const { permissions } = useCurrentMember();
  return (
    <Tabs screenOptions={screenOptions}>
      {tabDefinitions.map((tab) => (
        <Tabs.Screen
          key={tab.name}
          name={tab.name}
          options={{
            title: translate(tab.titleKey),
            href: isTabVisible(tab, permissions) ? undefined : null,
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

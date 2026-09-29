import { Tabs } from "expo-router";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { FamilyCartProvider, useFamilyCart } from "@/features/family/FamilyCartProvider";
import { FamilyStudentProvider } from "@/features/family/FamilyStudentProvider";
import {
  familyHiddenRouteNames,
  familyShopTabName,
  familyTabDefinitions,
} from "@/features/family/familyTabDefinitions";
import { translate } from "@/i18n/translate";
import { TabIcon, useTabBarScreenOptions } from "@/navigation/tabBar";

function FamilyTabs() {
  const screenOptions = useTabBarScreenOptions();
  const { units } = useFamilyCart();
  return (
    <Tabs screenOptions={screenOptions} backBehavior="history">
      {familyTabDefinitions.map((tab) => (
        <Tabs.Screen
          key={tab.name}
          name={tab.name}
          options={{
            title: translate(tab.titleKey),
            tabBarBadge: tab.name === familyShopTabName && units > 0 ? units : undefined,
            tabBarIcon: ({ focused }) => <TabIcon icon={tab.icon} isFocused={focused} />,
          }}
        />
      ))}
      {familyHiddenRouteNames.map((routeName) => (
        <Tabs.Screen key={routeName} name={routeName} options={{ href: null }} />
      ))}
    </Tabs>
  );
}

export default function FamilyLayout() {
  return (
    <RequireSessionKind kind="family">
      <FamilyStudentProvider>
        <FamilyCartProvider>
          <FamilyTabs />
        </FamilyCartProvider>
      </FamilyStudentProvider>
    </RequireSessionKind>
  );
}

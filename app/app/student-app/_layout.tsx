import { Tabs } from "expo-router";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { AccountLanguageSync } from "@/features/account/components/AccountLanguageSync";
import { LegalTermsGate } from "@/features/legal/components/LegalTermsGate";
import {
  StudentAppCartProvider,
  useStudentAppCart,
} from "@/features/studentApp/StudentAppCartProvider";
import { AccountStudentProvider } from "@/features/studentApp/AccountStudentProvider";
import {
  studentAppHiddenRouteNames,
  studentAppShopTabName,
  studentAppTabDefinitions,
} from "@/features/studentApp/studentAppTabDefinitions";
import { translate } from "@/i18n/translate";
import { TabIcon, tabAccessibilityLabel, useTabBarScreenOptions } from "@/navigation/tabBar";

function StudentAppTabs() {
  const screenOptions = useTabBarScreenOptions();
  const { units } = useStudentAppCart();
  return (
    <Tabs screenOptions={screenOptions} backBehavior="history">
      {studentAppTabDefinitions.map((tab) => (
        <Tabs.Screen
          key={tab.name}
          name={tab.name}
          options={({ navigation, route }) => ({
            title: translate(tab.titleKey),
            tabBarAccessibilityLabel: tabAccessibilityLabel(
              translate(tab.titleKey),
              navigation,
              route.key,
            ),
            tabBarBadge: tab.name === studentAppShopTabName && units > 0 ? units : undefined,
            tabBarIcon: ({ focused }) => <TabIcon icon={tab.icon} isFocused={focused} />,
          })}
        />
      ))}
      {studentAppHiddenRouteNames.map((routeName) => (
        <Tabs.Screen key={routeName} name={routeName} options={{ href: null }} />
      ))}
    </Tabs>
  );
}

export default function StudentAppLayout() {
  return (
    <RequireSessionKind kind="student">
      <AccountLanguageSync />
      <LegalTermsGate>
        <BrandProvider audience="student">
          <AccountStudentProvider>
            <StudentAppCartProvider>
              <StudentAppTabs />
            </StudentAppCartProvider>
          </AccountStudentProvider>
        </BrandProvider>
      </LegalTermsGate>
    </RequireSessionKind>
  );
}

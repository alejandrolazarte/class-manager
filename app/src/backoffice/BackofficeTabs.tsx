import { Tabs } from "expo-router";
import { BackofficeSessionGate } from "@/backoffice/session/BackofficeSessionGate";
import { translate, TranslationKey } from "@/i18n/translate";
import { IconName } from "@/ui/Icon";
import { TabIcon, tabAccessibilityLabel, useTabBarScreenOptions } from "@/ui/TabBar";

interface BackofficeTabDefinition {
  name: string;
  titleKey: TranslationKey;
  icon: IconName;
}

const backofficeTabDefinitions: readonly BackofficeTabDefinition[] = [
  { name: "index", titleKey: "backoffice.tabs.home", icon: "home" },
  { name: "brands", titleKey: "backoffice.tabs.brands", icon: "business" },
];

export function BackofficeTabs() {
  const screenOptions = useTabBarScreenOptions();
  return (
    <BackofficeSessionGate>
      <Tabs screenOptions={screenOptions}>
        {backofficeTabDefinitions.map((tab) => (
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
              tabBarIcon: ({ focused }) => <TabIcon icon={tab.icon} isFocused={focused} />,
            })}
          />
        ))}
      </Tabs>
    </BackofficeSessionGate>
  );
}

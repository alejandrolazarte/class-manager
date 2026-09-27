import { Tabs } from "expo-router";
import { View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { RedirectWhenSignedOut } from "@/features/authentication/components/RedirectWhenSignedOut";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { translate, TranslationKey } from "@/i18n/translate";
import { fontFamilies } from "@/theme/typography";
import { useTheme } from "@/theme/useTheme";
import { Icon, IconName } from "@/ui/Icon";

interface TabDefinition {
  name: string;
  titleKey: TranslationKey;
  icon: IconName;
}

const tabDefinitions: readonly TabDefinition[] = [
  { name: "today", titleKey: "tabs.today", icon: "today" },
  { name: "classes", titleKey: "tabs.classes", icon: "classes" },
  { name: "students", titleKey: "tabs.students", icon: "students" },
  { name: "fees", titleKey: "tabs.fees", icon: "fees" },
  { name: "settings", titleKey: "tabs.settings", icon: "settings" },
];

const tabBarContentHeight = 66;
const tabBarTopPadding = 10;
const tabBarMinimumBottomPadding = 12;
const tabLabelFontSize = 12;

function TabIcon({ icon, isFocused }: { icon: IconName; isFocused: boolean }) {
  return (
    <View
      className={`h-8 w-[60px] items-center justify-center rounded-2xl ${isFocused ? "bg-primary-soft" : ""}`}
    >
      <Icon
        name={icon}
        size="extraLarge"
        tone={isFocused ? "primary-soft-foreground" : "muted-foreground"}
      />
    </View>
  );
}

export default function TabsLayout() {
  const { colors } = useTheme();
  const { bottom } = useSafeAreaInsets();
  const bottomPadding = Math.max(bottom, tabBarMinimumBottomPadding);
  return (
    <RedirectWhenSignedOut>
      <BusinessProvider>
        <Tabs
          screenOptions={{
            headerShown: false,
            tabBarActiveTintColor: colors.foreground,
            tabBarInactiveTintColor: colors["muted-foreground"],
            tabBarStyle: {
              backgroundColor: colors.surface,
              borderTopColor: colors.border,
              height: tabBarContentHeight + bottomPadding,
              paddingTop: tabBarTopPadding,
              paddingBottom: bottomPadding,
            },
            tabBarLabelStyle: {
              fontFamily: fontFamilies.strong,
              fontSize: tabLabelFontSize,
              marginTop: 4,
            },
          }}
        >
          {tabDefinitions.map((tab) => (
            <Tabs.Screen
              key={tab.name}
              name={tab.name}
              options={{
                title: translate(tab.titleKey),
                tabBarIcon: ({ focused }) => <TabIcon icon={tab.icon} isFocused={focused} />,
              }}
            />
          ))}
        </Tabs>
      </BusinessProvider>
    </RedirectWhenSignedOut>
  );
}

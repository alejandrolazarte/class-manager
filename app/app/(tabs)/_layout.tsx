import { Tabs } from "expo-router";
import { View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { MemberProvider } from "@/features/members/MemberProvider";
import { translate } from "@/i18n/translate";
import { isTabVisible, tabDefinitions } from "@/navigation/tabDefinitions";
import { fontFamilies } from "@/theme/typography";
import { useTheme } from "@/theme/useTheme";
import { Icon, IconName } from "@/ui/Icon";

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

function MemberTabs() {
  const { colors } = useTheme();
  const { bottom } = useSafeAreaInsets();
  const { permissions } = useCurrentMember();
  const bottomPadding = Math.max(bottom, tabBarMinimumBottomPadding);
  return (
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
          <MemberTabs />
        </MemberProvider>
      </BusinessProvider>
    </RequireSessionKind>
  );
}

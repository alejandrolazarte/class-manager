import { Tabs } from "expo-router";
import { ComponentProps } from "react";
import { Platform, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { fontFamilies, typeScale } from "@/theme/typography";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { Icon, IconName, iconSizes } from "@/ui/Icon";
import { Entrance } from "@/ui/motion/Entrance";

type TabScreenOptions = Exclude<
  NonNullable<ComponentProps<typeof Tabs.Screen>["options"]>,
  (...parameters: never[]) => unknown
>;

interface TabNavigatorState {
  getState: () => { routes: readonly { key: string }[] };
}

const tabBarContentHeight = 64;
const tabBarTopPadding = 7;
const tabBarMinimumBottomPadding = 12;
const activeMarkClassName = "h-1 w-1 rounded-full";
const iosTabRoleDescription = "tab";
const iosTabPositionSeparator = "of";

export function TabIcon({ icon, isFocused }: { icon: IconName; isFocused: boolean }) {
  return <Icon name={icon} size="large" tone={isFocused ? "primary" : "muted-foreground"} />;
}

export function TabLabel({ label, isFocused }: { label: string; isFocused: boolean }) {
  return (
    <View className="mt-[3px] items-center gap-[3px]">
      <AppText variant="tab" tone={isFocused ? "primary" : "muted"} numberOfLines={1}>
        {label}
      </AppText>
      {isFocused ? (
        <Entrance kind="pop">
          <View testID="tab-active-mark" className={`${activeMarkClassName} bg-primary`} />
        </Entrance>
      ) : (
        <View className={activeMarkClassName} />
      )}
    </View>
  );
}

export function tabAccessibilityLabel(
  title: string,
  navigation: TabNavigatorState,
  routeKey: string,
): string | undefined {
  if (Platform.OS !== "ios") {
    return undefined;
  }
  const { routes } = navigation.getState();
  const position = routes.findIndex((route) => route.key === routeKey) + 1;
  return `${title}, ${iosTabRoleDescription}, ${position} ${iosTabPositionSeparator} ${routes.length}`;
}

export function useTabBarScreenOptions(): TabScreenOptions {
  const { colors } = useTheme();
  const { bottom } = useSafeAreaInsets();
  const bottomPadding = Math.max(bottom, tabBarMinimumBottomPadding);
  return {
    headerShown: false,
    tabBarActiveTintColor: colors.primary,
    tabBarInactiveTintColor: colors["muted-foreground"],
    tabBarStyle: {
      backgroundColor: colors.surface,
      borderTopColor: colors["border-subtle"],
      height: tabBarContentHeight + bottomPadding,
      paddingTop: tabBarTopPadding,
      paddingBottom: bottomPadding,
    },
    tabBarIconStyle: { height: iconSizes.large },
    tabBarLabel: ({ focused, children }) => <TabLabel label={children} isFocused={focused} />,
    tabBarBadgeStyle: {
      backgroundColor: colors.danger,
      color: colors["danger-foreground"],
      fontFamily: fontFamilies.heavy,
      fontSize: typeScale.micro.fontSize,
    },
  };
}

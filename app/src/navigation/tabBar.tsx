import { Tabs } from "expo-router";
import { ComponentProps } from "react";
import { View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { fontFamilies, typeScale } from "@/theme/typography";
import { useTheme } from "@/theme/useTheme";
import { Icon, IconName } from "@/ui/Icon";

type TabScreenOptions = Exclude<
  NonNullable<ComponentProps<typeof Tabs.Screen>["options"]>,
  (...parameters: never[]) => unknown
>;

const tabBarContentHeight = 66;
const tabBarTopPadding = 10;
const tabBarMinimumBottomPadding = 12;
const tabLabelTopMargin = 4;

export function TabIcon({ icon, isFocused }: { icon: IconName; isFocused: boolean }) {
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

export function useTabBarScreenOptions(): TabScreenOptions {
  const { colors } = useTheme();
  const { bottom } = useSafeAreaInsets();
  const bottomPadding = Math.max(bottom, tabBarMinimumBottomPadding);
  return {
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
      fontSize: typeScale.small.fontSize,
      marginTop: tabLabelTopMargin,
    },
    tabBarBadgeStyle: {
      backgroundColor: colors.danger,
      color: colors["danger-foreground"],
      fontFamily: fontFamilies.strong,
      fontSize: typeScale.micro.fontSize,
    },
  };
}

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

const tabBarContentHeight = 64;
const tabBarTopPadding = 6;
const tabBarMinimumBottomPadding = 12;
const tabLabelTopMargin = 4;
const activeMarkClassName = "h-1 w-1 rounded-full";

export function TabIcon({ icon, isFocused }: { icon: IconName; isFocused: boolean }) {
  return (
    <View className="items-center gap-[3px]">
      <Icon name={icon} size="large" tone={isFocused ? "primary" : "muted-foreground"} />
      {isFocused ? (
        <View testID="tab-active-mark" className={`${activeMarkClassName} bg-primary`} />
      ) : (
        <View className={activeMarkClassName} />
      )}
    </View>
  );
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
    tabBarLabelStyle: {
      fontFamily: fontFamilies.label,
      fontSize: typeScale.micro.fontSize,
      marginTop: tabLabelTopMargin,
    },
    tabBarBadgeStyle: {
      backgroundColor: colors.danger,
      color: colors["danger-foreground"],
      fontFamily: fontFamilies.heavy,
      fontSize: typeScale.micro.fontSize,
    },
  };
}

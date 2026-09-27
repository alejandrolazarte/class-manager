import { DarkTheme, DefaultTheme } from "expo-router";
import { ThemeColors } from "@/theme/themeColorTokens";
import { ColorScheme } from "@/theme/themes";

export function buildNavigationTheme(colors: ThemeColors, colorScheme: ColorScheme) {
  const baseNavigationTheme = colorScheme === "dark" ? DarkTheme : DefaultTheme;
  return {
    ...baseNavigationTheme,
    colors: {
      primary: colors.primary,
      background: colors.background,
      card: colors.surface,
      text: colors.foreground,
      border: colors.border,
      notification: colors.danger,
    },
  };
}

import { ThemeProvider as NavigationThemeProvider } from "expo-router";
import { ReactNode, useMemo, useState } from "react";
import { useColorScheme } from "react-native";
import { buildNavigationTheme } from "@/theme/buildNavigationTheme";
import { ColorSchemePreference, ThemeContext, ThemeContextValue } from "@/theme/ThemeContext";
import { ThemeScope } from "@/theme/ThemeScope";
import {
  brandThemeName,
  ColorScheme,
  defaultThemeName,
  ThemeDefinition,
  ThemeName,
  ThemeRegistry,
  themes,
} from "@/theme/themes";

interface ThemeProviderProps {
  children: ReactNode;
  customThemes?: ThemeRegistry;
  baseThemeName?: ThemeName;
  initialColorSchemePreference?: ColorSchemePreference;
}

function resolveColorScheme(
  preference: ColorSchemePreference,
  systemColorScheme: string | null | undefined,
): ColorScheme {
  if (preference !== "system") {
    return preference;
  }
  return systemColorScheme === "dark" ? "dark" : "light";
}

export function ThemeProvider({
  children,
  customThemes,
  baseThemeName = defaultThemeName,
  initialColorSchemePreference = "system",
}: ThemeProviderProps) {
  const [brandTheme, setBrandTheme] = useState<ThemeDefinition | null>(null);
  const [colorSchemePreference, setColorSchemePreference] = useState<ColorSchemePreference>(
    initialColorSchemePreference,
  );
  const registry = useMemo<ThemeRegistry>(() => ({ ...themes, ...customThemes }), [customThemes]);
  const themeName =
    brandTheme !== null
      ? brandThemeName
      : baseThemeName in registry
        ? baseThemeName
        : defaultThemeName;
  const theme = brandTheme ?? registry[themeName]!;
  const colorScheme = resolveColorScheme(colorSchemePreference, useColorScheme());
  const colors = theme[colorScheme];

  const navigationTheme = useMemo(
    () => buildNavigationTheme(colors, colorScheme),
    [colors, colorScheme],
  );
  const themeContextValue = useMemo<ThemeContextValue>(
    () => ({
      themeName,
      colorScheme,
      colorSchemePreference,
      colors,
      setBrandTheme,
      setColorSchemePreference,
    }),
    [themeName, colorScheme, colorSchemePreference, colors],
  );

  return (
    <ThemeContext.Provider value={themeContextValue}>
      <NavigationThemeProvider value={navigationTheme}>
        <ThemeScope className="flex-1 bg-background">{children}</ThemeScope>
      </NavigationThemeProvider>
    </ThemeContext.Provider>
  );
}

import { ThemeProvider as NavigationThemeProvider } from "expo-router";
import { ReactNode, useCallback, useMemo, useState } from "react";
import { useColorScheme } from "react-native";
import { buildNavigationTheme } from "@/theme/buildNavigationTheme";
import { ColorSchemePreference, ThemeContext, ThemeContextValue } from "@/theme/ThemeContext";
import { ThemeScope } from "@/theme/ThemeScope";
import { ColorScheme, defaultThemeName, ThemeName, ThemeRegistry, themes } from "@/theme/themes";

interface ThemeProviderProps {
  children: ReactNode;
  customThemes?: ThemeRegistry;
  initialThemeName?: ThemeName;
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
  initialThemeName = defaultThemeName,
  initialColorSchemePreference = "system",
}: ThemeProviderProps) {
  const registry = useMemo<ThemeRegistry>(() => ({ ...themes, ...customThemes }), [customThemes]);
  const [requestedThemeName, setRequestedThemeName] = useState<ThemeName>(initialThemeName);
  const [colorSchemePreference, setColorSchemePreference] = useState<ColorSchemePreference>(
    initialColorSchemePreference,
  );
  const themeName = requestedThemeName in registry ? requestedThemeName : defaultThemeName;
  const colorScheme = resolveColorScheme(colorSchemePreference, useColorScheme());
  const colors = registry[themeName]![colorScheme];

  const previewColors = useCallback(
    (candidateThemeName: ThemeName) =>
      (registry[candidateThemeName] ?? registry[defaultThemeName]!)[colorScheme],
    [registry, colorScheme],
  );
  const navigationTheme = useMemo(
    () => buildNavigationTheme(colors, colorScheme),
    [colors, colorScheme],
  );
  const themeContextValue = useMemo<ThemeContextValue>(
    () => ({
      themeName,
      themeNames: Object.keys(registry),
      colorScheme,
      colorSchemePreference,
      colors,
      previewColors,
      setThemeName: setRequestedThemeName,
      setColorSchemePreference,
    }),
    [themeName, registry, colorScheme, colorSchemePreference, colors, previewColors],
  );

  return (
    <ThemeContext.Provider value={themeContextValue}>
      <NavigationThemeProvider value={navigationTheme}>
        <ThemeScope className="flex-1 bg-background">{children}</ThemeScope>
      </NavigationThemeProvider>
    </ThemeContext.Provider>
  );
}

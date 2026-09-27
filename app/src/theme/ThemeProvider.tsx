import { ThemeProvider as NavigationThemeProvider } from "expo-router";
import { vars } from "nativewind";
import { ReactNode, useMemo, useState } from "react";
import { useColorScheme, View } from "react-native";
import { buildNavigationTheme } from "@/theme/buildNavigationTheme";
import { buildThemeVariables } from "@/theme/buildThemeVariables";
import { ColorSchemePreference, ThemeContext, ThemeContextValue } from "@/theme/ThemeContext";
import { ColorScheme, defaultThemeName, ThemeName, themes } from "@/theme/themes";

interface ThemeProviderProps {
  children: ReactNode;
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
  initialThemeName = defaultThemeName,
  initialColorSchemePreference = "system",
}: ThemeProviderProps) {
  const [themeName, setThemeName] = useState<ThemeName>(initialThemeName);
  const [colorSchemePreference, setColorSchemePreference] = useState<ColorSchemePreference>(
    initialColorSchemePreference,
  );
  const colorScheme = resolveColorScheme(colorSchemePreference, useColorScheme());
  const colors = themes[themeName][colorScheme];

  const themeVariablesStyle = useMemo(() => vars(buildThemeVariables(colors)), [colors]);
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
      setThemeName,
      setColorSchemePreference,
    }),
    [themeName, colorScheme, colorSchemePreference, colors],
  );

  return (
    <ThemeContext.Provider value={themeContextValue}>
      <NavigationThemeProvider value={navigationTheme}>
        <View style={themeVariablesStyle} className="flex-1 bg-background">
          {children}
        </View>
      </NavigationThemeProvider>
    </ThemeContext.Provider>
  );
}

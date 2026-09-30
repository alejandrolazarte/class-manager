import { ThemeProvider as NavigationThemeProvider } from "expo-router";
import { ReactNode, useCallback, useMemo, useState } from "react";
import { useColorScheme } from "react-native";
import { buildNavigationTheme } from "@/theme/buildNavigationTheme";
import {
  BrandTheme,
  ColorSchemePreference,
  ThemeContext,
  ThemeContextValue,
} from "@/theme/ThemeContext";
import { ThemeScope } from "@/theme/ThemeScope";
import {
  brandThemeName,
  ColorScheme,
  defaultThemeName,
  ThemeName,
  ThemeRegistry,
  themes,
} from "@/theme/themes";

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

function resolveThemeName(
  chosenThemeName: ThemeName | null,
  brandTheme: BrandTheme | null,
  registry: ThemeRegistry,
): ThemeName {
  if (brandTheme?.isLocked) {
    return brandThemeName;
  }
  if (chosenThemeName !== null && chosenThemeName in registry) {
    return chosenThemeName;
  }
  return brandTheme === null ? defaultThemeName : brandThemeName;
}

export function ThemeProvider({
  children,
  customThemes,
  initialThemeName,
  initialColorSchemePreference = "system",
}: ThemeProviderProps) {
  const [brandTheme, setBrandTheme] = useState<BrandTheme | null>(null);
  const registry = useMemo<ThemeRegistry>(
    () => ({
      ...themes,
      ...customThemes,
      ...(brandTheme === null ? {} : { [brandThemeName]: brandTheme.theme }),
    }),
    [customThemes, brandTheme],
  );
  const [chosenThemeName, setChosenThemeName] = useState<ThemeName | null>(
    initialThemeName ?? null,
  );
  const [colorSchemePreference, setColorSchemePreference] = useState<ColorSchemePreference>(
    initialColorSchemePreference,
  );
  const themeName = resolveThemeName(chosenThemeName, brandTheme, registry);
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
      chosenThemeName,
      isThemeLocked: brandTheme?.isLocked ?? false,
      themeNames: Object.keys(registry),
      colorScheme,
      colorSchemePreference,
      colors,
      previewColors,
      setThemeName: setChosenThemeName,
      setBrandTheme,
      setColorSchemePreference,
    }),
    [
      themeName,
      chosenThemeName,
      brandTheme,
      registry,
      colorScheme,
      colorSchemePreference,
      colors,
      previewColors,
    ],
  );

  return (
    <ThemeContext.Provider value={themeContextValue}>
      <NavigationThemeProvider value={navigationTheme}>
        <ThemeScope className="flex-1 bg-background">{children}</ThemeScope>
      </NavigationThemeProvider>
    </ThemeContext.Provider>
  );
}

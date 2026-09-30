import { createContext } from "react";
import { ThemeColors } from "@/theme/themeColorTokens";
import { ColorScheme, ThemeDefinition, ThemeName } from "@/theme/themes";

export type ColorSchemePreference = ColorScheme | "system";

export interface BrandTheme {
  theme: ThemeDefinition;
  isLocked: boolean;
}

export interface ThemeContextValue {
  themeName: ThemeName;
  chosenThemeName: ThemeName | null;
  isThemeLocked: boolean;
  themeNames: ThemeName[];
  colorScheme: ColorScheme;
  colorSchemePreference: ColorSchemePreference;
  colors: ThemeColors;
  previewColors: (themeName: ThemeName) => ThemeColors;
  setThemeName: (themeName: ThemeName | null) => void;
  setBrandTheme: (brandTheme: BrandTheme | null) => void;
  setColorSchemePreference: (preference: ColorSchemePreference) => void;
}

export const ThemeContext = createContext<ThemeContextValue | null>(null);

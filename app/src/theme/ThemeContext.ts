import { createContext } from "react";
import { ThemeColors } from "@/theme/themeColorTokens";
import { ColorScheme, ThemeName } from "@/theme/themes";

export type ColorSchemePreference = ColorScheme | "system";

export interface ThemeContextValue {
  themeName: ThemeName;
  themeNames: ThemeName[];
  colorScheme: ColorScheme;
  colorSchemePreference: ColorSchemePreference;
  colors: ThemeColors;
  previewColors: (themeName: ThemeName) => ThemeColors;
  setThemeName: (themeName: ThemeName) => void;
  setColorSchemePreference: (preference: ColorSchemePreference) => void;
}

export const ThemeContext = createContext<ThemeContextValue | null>(null);

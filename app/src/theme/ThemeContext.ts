import { createContext } from "react";
import { ThemeColors } from "@/theme/themeColorTokens";
import { ColorScheme, ThemeDefinition, ThemeName } from "@/theme/themes";

export type ColorSchemePreference = ColorScheme | "system";

export interface ThemeContextValue {
  themeName: ThemeName;
  colorScheme: ColorScheme;
  colorSchemePreference: ColorSchemePreference;
  colors: ThemeColors;
  setBrandTheme: (brandTheme: ThemeDefinition | null) => void;
  setColorSchemePreference: (preference: ColorSchemePreference) => void;
}

export const ThemeContext = createContext<ThemeContextValue | null>(null);

import { palette } from "@/theme/palette";
import { ThemeColors } from "@/theme/themeColorTokens";

export const colorSchemes = ["light", "dark"] as const;

export type ColorScheme = (typeof colorSchemes)[number];

export type ThemeDefinition = Record<ColorScheme, ThemeColors>;

const neutralLightColors = {
  background: palette.gray50,
  foreground: palette.gray900,
  surface: palette.white,
  muted: palette.gray100,
  "muted-foreground": palette.gray600,
  "subtle-foreground": palette.gray500,
  "disabled-foreground": palette.gray400,
  border: palette.gray200,
  "border-subtle": palette.gray100,
  "border-strong": palette.gray300,
  danger: palette.red600,
  "danger-foreground": palette.white,
  "danger-soft": palette.red50,
  "danger-soft-foreground": palette.red800,
  warning: palette.amber700,
  "warning-soft": palette.amber100,
  "warning-soft-foreground": palette.amber800,
  success: palette.green700,
  "success-soft": palette.green100,
  "success-soft-foreground": palette.green800,
  inverse: palette.gray900,
  "inverse-foreground": palette.white,
} as const;

const neutralDarkColors = {
  background: palette.gray950,
  foreground: palette.gray100,
  surface: palette.gray900,
  muted: palette.gray800,
  "muted-foreground": palette.gray300,
  "subtle-foreground": palette.gray400,
  "disabled-foreground": palette.gray600,
  border: palette.gray800,
  "border-subtle": palette.gray800,
  "border-strong": palette.gray700,
  danger: palette.red400,
  "danger-foreground": palette.gray950,
  "danger-soft": palette.red950,
  "danger-soft-foreground": palette.red300,
  warning: palette.amber300,
  "warning-soft": palette.amber950,
  "warning-soft-foreground": palette.amber300,
  success: palette.green300,
  "success-soft": palette.green950,
  "success-soft-foreground": palette.green300,
  inverse: palette.gray100,
  "inverse-foreground": palette.gray900,
} as const;

const violetTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.violet600,
    "primary-foreground": palette.white,
    "primary-strong": palette.violet800,
    "primary-soft": palette.violet100,
    "primary-soft-foreground": palette.violet800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.violet300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.violet200,
    "primary-soft": palette.violet950,
    "primary-soft-foreground": palette.violet200,
  },
};

const oceanTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.sky700,
    "primary-foreground": palette.white,
    "primary-strong": palette.sky800,
    "primary-soft": palette.sky100,
    "primary-soft-foreground": palette.sky800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.sky300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.sky200,
    "primary-soft": palette.sky950,
    "primary-soft-foreground": palette.sky200,
  },
};

export const themes = {
  violet: violetTheme,
  ocean: oceanTheme,
} as const satisfies Record<string, ThemeDefinition>;

export type ThemeName = keyof typeof themes;

export const defaultThemeName: ThemeName = "violet";

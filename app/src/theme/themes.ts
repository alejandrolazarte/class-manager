import { palette } from "@/theme/palette";
import { ThemeColors } from "@/theme/themeColorTokens";

export const colorSchemes = ["light", "dark"] as const;

export type ColorScheme = (typeof colorSchemes)[number];

export type ThemeDefinition = Record<ColorScheme, ThemeColors>;

export type ThemeName = string;

export type ThemeRegistry = Readonly<Record<ThemeName, ThemeDefinition>>;

export const neutralLightColors = {
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
  "success-foreground": palette.white,
  "success-soft": palette.green100,
  "success-soft-foreground": palette.green800,
  inverse: palette.gray900,
  "inverse-foreground": palette.white,
  shadow: palette.gray900,
} as const;

export const neutralDarkColors = {
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
  "success-foreground": palette.gray950,
  "success-soft": palette.green950,
  "success-soft-foreground": palette.green300,
  inverse: palette.gray100,
  "inverse-foreground": palette.gray900,
  shadow: palette.gray950,
} as const;

const aquaTheme: ThemeDefinition = {
  light: {
    background: palette.aqua50,
    foreground: palette.aqua900,
    surface: palette.white,
    muted: palette.aqua150,
    "muted-foreground": palette.aqua600,
    "subtle-foreground": palette.aqua500,
    "disabled-foreground": palette.aqua400,
    border: palette.aqua200,
    "border-subtle": palette.aqua100,
    "border-strong": palette.aqua300,
    primary: palette.lagoon600,
    "primary-foreground": palette.white,
    "primary-strong": palette.lagoon700,
    "primary-soft": palette.lagoon100,
    "primary-soft-foreground": palette.lagoon800,
    accent: palette.lagoon600,
    "accent-soft": palette.lagoon100,
    "accent-soft-foreground": palette.lagoon800,
    danger: palette.coral600,
    "danger-foreground": palette.white,
    "danger-soft": palette.coral100,
    "danger-soft-foreground": palette.coral800,
    warning: palette.sand600,
    "warning-soft": palette.sand100,
    "warning-soft-foreground": palette.sand800,
    success: palette.seaGreen600,
    "success-foreground": palette.white,
    "success-soft": palette.seaGreen100,
    "success-soft-foreground": palette.seaGreen800,
    inverse: palette.aqua900,
    "inverse-foreground": palette.aqua50,
    shadow: palette.aquaShadow,
  },
  dark: {
    background: palette.aquaNight950,
    foreground: palette.aquaNight50,
    surface: palette.aquaNight900,
    muted: palette.aquaNight850,
    "muted-foreground": palette.aquaNight200,
    "subtle-foreground": palette.aquaNight400,
    "disabled-foreground": palette.aquaNight600,
    border: palette.aquaNight800,
    "border-subtle": palette.aquaNight875,
    "border-strong": palette.aquaNight700,
    primary: palette.lagoon400,
    "primary-foreground": palette.lagoon975,
    "primary-strong": palette.lagoon300,
    "primary-soft": palette.lagoon900,
    "primary-soft-foreground": palette.lagoon200,
    accent: palette.lagoon400,
    "accent-soft": palette.lagoon900,
    "accent-soft-foreground": palette.lagoon200,
    danger: palette.coral400,
    "danger-foreground": palette.lagoon975,
    "danger-soft": palette.coral900,
    "danger-soft-foreground": palette.coral200,
    warning: palette.sand400,
    "warning-soft": palette.sand900,
    "warning-soft-foreground": palette.sand200,
    success: palette.seaGreen400,
    "success-foreground": palette.lagoon975,
    "success-soft": palette.seaGreen900,
    "success-soft-foreground": palette.seaGreen200,
    inverse: palette.aquaNight50,
    "inverse-foreground": palette.aquaNight950,
    shadow: palette.aquaNightShadow,
  },
};

const violetTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.violet600,
    "primary-foreground": palette.white,
    "primary-strong": palette.violet800,
    "primary-soft": palette.violet100,
    "primary-soft-foreground": palette.violet800,
    accent: palette.violet600,
    "accent-soft": palette.violet100,
    "accent-soft-foreground": palette.violet800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.violet300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.violet200,
    "primary-soft": palette.violet950,
    "primary-soft-foreground": palette.violet200,
    accent: palette.violet300,
    "accent-soft": palette.violet950,
    "accent-soft-foreground": palette.violet200,
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
    accent: palette.sky700,
    "accent-soft": palette.sky100,
    "accent-soft-foreground": palette.sky800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.sky300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.sky200,
    "primary-soft": palette.sky950,
    "primary-soft-foreground": palette.sky200,
    accent: palette.sky300,
    "accent-soft": palette.sky950,
    "accent-soft-foreground": palette.sky200,
  },
};

const roseTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.magenta700,
    "primary-foreground": palette.white,
    "primary-strong": palette.magenta800,
    "primary-soft": palette.magenta100,
    "primary-soft-foreground": palette.magenta800,
    accent: palette.magenta700,
    "accent-soft": palette.magenta100,
    "accent-soft-foreground": palette.magenta800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.magenta300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.magenta200,
    "primary-soft": palette.magenta950,
    "primary-soft-foreground": palette.magenta200,
    accent: palette.magenta300,
    "accent-soft": palette.magenta950,
    "accent-soft-foreground": palette.magenta200,
  },
};

const plumTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.fuchsia700,
    "primary-foreground": palette.white,
    "primary-strong": palette.fuchsia800,
    "primary-soft": palette.fuchsia100,
    "primary-soft-foreground": palette.fuchsia800,
    accent: palette.fuchsia700,
    "accent-soft": palette.fuchsia100,
    "accent-soft-foreground": palette.fuchsia800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.fuchsia300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.fuchsia200,
    "primary-soft": palette.fuchsia950,
    "primary-soft-foreground": palette.fuchsia200,
    accent: palette.fuchsia300,
    "accent-soft": palette.fuchsia950,
    "accent-soft-foreground": palette.fuchsia200,
  },
};

const indigoTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.indigo600,
    "primary-foreground": palette.white,
    "primary-strong": palette.indigo800,
    "primary-soft": palette.indigo100,
    "primary-soft-foreground": palette.indigo800,
    accent: palette.indigo600,
    "accent-soft": palette.indigo100,
    "accent-soft-foreground": palette.indigo800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.indigo300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.indigo200,
    "primary-soft": palette.indigo950,
    "primary-soft-foreground": palette.indigo200,
    accent: palette.indigo300,
    "accent-soft": palette.indigo950,
    "accent-soft-foreground": palette.indigo200,
  },
};

const graphiteTheme: ThemeDefinition = {
  light: {
    ...neutralLightColors,
    primary: palette.slate700,
    "primary-foreground": palette.white,
    "primary-strong": palette.slate800,
    "primary-soft": palette.slate100,
    "primary-soft-foreground": palette.slate800,
    accent: palette.slate700,
    "accent-soft": palette.slate100,
    "accent-soft-foreground": palette.slate800,
  },
  dark: {
    ...neutralDarkColors,
    primary: palette.slate300,
    "primary-foreground": palette.gray950,
    "primary-strong": palette.slate200,
    "primary-soft": palette.slate800,
    "primary-soft-foreground": palette.slate200,
    accent: palette.slate300,
    "accent-soft": palette.slate800,
    "accent-soft-foreground": palette.slate200,
  },
};

export const themes = {
  aqua: aquaTheme,
  violet: violetTheme,
  ocean: oceanTheme,
  rose: roseTheme,
  plum: plumTheme,
  indigo: indigoTheme,
  graphite: graphiteTheme,
} as const satisfies ThemeRegistry;

export type BuiltInThemeName = keyof typeof themes;

export const defaultThemeName: BuiltInThemeName = "aqua";

export const brandThemeName = "business";

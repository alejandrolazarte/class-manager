export const themeColorTokens = [
  "background",
  "foreground",
  "surface",
  "muted",
  "muted-foreground",
  "subtle-foreground",
  "disabled-foreground",
  "border",
  "border-subtle",
  "border-strong",
  "primary",
  "primary-foreground",
  "primary-strong",
  "primary-soft",
  "primary-soft-foreground",
  "danger",
  "danger-foreground",
  "danger-soft",
  "danger-soft-foreground",
  "warning",
  "warning-soft",
  "warning-soft-foreground",
  "success",
  "success-foreground",
  "success-soft",
  "success-soft-foreground",
  "inverse",
  "inverse-foreground",
  "shadow",
] as const;

export type ThemeColorToken = (typeof themeColorTokens)[number];

export type ThemeColors = Record<ThemeColorToken, string>;

interface ThemeContrastPair {
  foreground: ThemeColorToken;
  background: ThemeColorToken;
}

export const themeContrastPairs: readonly ThemeContrastPair[] = [
  { foreground: "foreground", background: "background" },
  { foreground: "foreground", background: "surface" },
  { foreground: "foreground", background: "muted" },
  { foreground: "muted-foreground", background: "background" },
  { foreground: "muted-foreground", background: "surface" },
  { foreground: "subtle-foreground", background: "surface" },
  { foreground: "primary", background: "background" },
  { foreground: "primary", background: "surface" },
  { foreground: "primary-foreground", background: "primary" },
  { foreground: "primary-foreground", background: "primary-strong" },
  { foreground: "primary-soft-foreground", background: "primary-soft" },
  { foreground: "danger", background: "background" },
  { foreground: "danger", background: "surface" },
  { foreground: "danger-foreground", background: "danger" },
  { foreground: "danger-soft-foreground", background: "danger-soft" },
  { foreground: "foreground", background: "danger-soft" },
  { foreground: "warning", background: "surface" },
  { foreground: "warning-soft-foreground", background: "warning-soft" },
  { foreground: "foreground", background: "warning-soft" },
  { foreground: "success", background: "surface" },
  { foreground: "success-foreground", background: "success" },
  { foreground: "success-soft-foreground", background: "success-soft" },
  { foreground: "inverse-foreground", background: "inverse" },
];

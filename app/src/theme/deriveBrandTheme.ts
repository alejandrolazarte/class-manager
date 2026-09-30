import { contrastRatio } from "@/theme/contrastRatio";
import { mixColors } from "@/theme/mixColors";
import { palette } from "@/theme/palette";
import { ThemeColors } from "@/theme/themeColorTokens";
import { minimumTextContrastRatio } from "@/theme/themeContrast";
import { neutralDarkColors, neutralLightColors, ThemeDefinition } from "@/theme/themes";

export interface BrandColors {
  themeColor: string;
  accentColor: string | null;
}

const adjustmentStep = 0.06;
const maximumAdjustmentSteps = Math.ceil(1 / adjustmentStep);
const strongShadeWeight = 0.2;
const lightSoftTintWeight = 0.85;
const darkSoftShadeWeight = 0.75;

function adjustUntilReadable(
  color: string,
  towardColor: string,
  againstColors: readonly string[],
): string {
  for (let step = 0; step <= maximumAdjustmentSteps; step += 1) {
    const candidate = mixColors(color, towardColor, Math.min(step * adjustmentStep, 1));
    if (
      againstColors.every(
        (against) => contrastRatio(candidate, against) >= minimumTextContrastRatio,
      )
    ) {
      return candidate;
    }
  }
  return towardColor;
}

type BrandTokens = Pick<
  ThemeColors,
  | "primary"
  | "primary-foreground"
  | "primary-strong"
  | "primary-soft"
  | "primary-soft-foreground"
  | "accent"
  | "accent-soft"
  | "accent-soft-foreground"
>;

function deriveLightTokens(themeColor: string, accentColor: string): BrandTokens {
  const foreground = palette.white;
  const backgrounds = [neutralLightColors.background, neutralLightColors.surface, foreground];
  const primary = adjustUntilReadable(themeColor, palette.black, backgrounds);
  const primarySoft = mixColors(themeColor, palette.white, lightSoftTintWeight);
  const accentSoft = mixColors(accentColor, palette.white, lightSoftTintWeight);
  return {
    primary,
    "primary-foreground": foreground,
    "primary-strong": mixColors(primary, palette.black, strongShadeWeight),
    "primary-soft": primarySoft,
    "primary-soft-foreground": adjustUntilReadable(primary, palette.black, [primarySoft]),
    accent: adjustUntilReadable(accentColor, palette.black, backgrounds),
    "accent-soft": accentSoft,
    "accent-soft-foreground": adjustUntilReadable(accentColor, palette.black, [accentSoft]),
  };
}

function deriveDarkTokens(themeColor: string, accentColor: string): BrandTokens {
  const foreground = palette.gray950;
  const backgrounds = [neutralDarkColors.background, neutralDarkColors.surface, foreground];
  const primary = adjustUntilReadable(themeColor, palette.white, backgrounds);
  const primarySoft = mixColors(themeColor, palette.gray950, darkSoftShadeWeight);
  const accentSoft = mixColors(accentColor, palette.gray950, darkSoftShadeWeight);
  return {
    primary,
    "primary-foreground": foreground,
    "primary-strong": mixColors(primary, palette.white, strongShadeWeight),
    "primary-soft": primarySoft,
    "primary-soft-foreground": adjustUntilReadable(primary, palette.white, [primarySoft]),
    accent: adjustUntilReadable(accentColor, palette.white, backgrounds),
    "accent-soft": accentSoft,
    "accent-soft-foreground": adjustUntilReadable(accentColor, palette.white, [accentSoft]),
  };
}

function withAccentFromPrimary(tokens: BrandTokens): BrandTokens {
  return {
    ...tokens,
    accent: tokens.primary,
    "accent-soft": tokens["primary-soft"],
    "accent-soft-foreground": tokens["primary-soft-foreground"],
  };
}

export function deriveBrandTheme({ themeColor, accentColor }: BrandColors): ThemeDefinition {
  const lightTokens = deriveLightTokens(themeColor, accentColor ?? themeColor);
  const darkTokens = deriveDarkTokens(themeColor, accentColor ?? themeColor);
  return {
    light: {
      ...neutralLightColors,
      ...(accentColor === null ? withAccentFromPrimary(lightTokens) : lightTokens),
    },
    dark: {
      ...neutralDarkColors,
      ...(accentColor === null ? withAccentFromPrimary(darkTokens) : darkTokens),
    },
  };
}

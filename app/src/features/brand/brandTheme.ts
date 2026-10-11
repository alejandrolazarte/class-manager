import { Brand } from "@/features/brand/types";
import { findBuiltInThemeName } from "@/theme/builtInThemeColors";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { parseThemeDefinition } from "@/theme/parseThemeDefinition";
import { ThemeDefinition, themes } from "@/theme/themes";

type BrandColorSettings = Pick<Brand, "themeColor" | "accentColor">;

export function toBrandTheme({
  themeColor,
  accentColor,
}: BrandColorSettings): ThemeDefinition | null {
  if (themeColor === null) {
    return null;
  }
  const builtInThemeName = findBuiltInThemeName(themeColor, accentColor);
  if (builtInThemeName !== null) {
    return themes[builtInThemeName];
  }
  const parsed = parseThemeDefinition(deriveBrandTheme({ themeColor, accentColor }));
  return parsed.isValid ? parsed.theme : null;
}

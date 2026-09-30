import { Brand } from "@/features/brand/types";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { parseThemeDefinition } from "@/theme/parseThemeDefinition";
import { BrandTheme } from "@/theme/ThemeContext";

type BrandColorSettings = Pick<Brand, "themeColor" | "accentColor" | "locksTheme">;

export function toBrandTheme({
  themeColor,
  accentColor,
  locksTheme,
}: BrandColorSettings): BrandTheme | null {
  if (themeColor === null) {
    return null;
  }
  const parsed = parseThemeDefinition(deriveBrandTheme({ themeColor, accentColor }));
  return parsed.isValid ? { theme: parsed.theme, isLocked: locksTheme } : null;
}

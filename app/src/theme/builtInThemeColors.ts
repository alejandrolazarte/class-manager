import { BuiltInThemeName, themes } from "@/theme/themes";

export const builtInThemeNames = Object.keys(themes) as BuiltInThemeName[];

export function builtInThemeColor(themeName: BuiltInThemeName): string {
  return themes[themeName].light.primary.toLowerCase();
}

export function findBuiltInThemeName(
  themeColor: string,
  accentColor: string | null,
): BuiltInThemeName | null {
  if (accentColor !== null) {
    return null;
  }
  const normalizedThemeColor = themeColor.toLowerCase();
  return (
    builtInThemeNames.find((themeName) => builtInThemeColor(themeName) === normalizedThemeColor) ??
    null
  );
}

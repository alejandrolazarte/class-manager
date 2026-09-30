import { z } from "zod";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { defaultThemeName, ThemeName } from "@/theme/themes";

export const themePreferenceStorageKey = "class-manager.theme-preferences";

export interface ThemePreferenceStorage {
  read: () => Promise<string | null>;
  write: (serializedPreferences: string) => Promise<void>;
}

export interface ThemePreferences {
  themeName: ThemeName | null;
  colorSchemePreference: ColorSchemePreference;
}

const themePreferencesSchema = z.object({
  themeName: z.string().nullable(),
  colorSchemePreference: z.enum(["light", "dark", "system"]),
  isThemeChosen: z.boolean().optional(),
});

export function parseThemePreferences(
  serializedPreferences: string | null,
): ThemePreferences | null {
  if (serializedPreferences === null) {
    return null;
  }
  try {
    const parsed = themePreferencesSchema.safeParse(JSON.parse(serializedPreferences));
    if (!parsed.success) {
      return null;
    }
    const { themeName, colorSchemePreference, isThemeChosen } = parsed.data;
    const wasSavedWithoutChoosing = isThemeChosen === undefined && themeName === defaultThemeName;
    return {
      themeName: wasSavedWithoutChoosing ? null : themeName,
      colorSchemePreference,
    };
  } catch {
    return null;
  }
}

export function serializeThemePreferences(preferences: ThemePreferences): string {
  return JSON.stringify({ ...preferences, isThemeChosen: preferences.themeName !== null });
}

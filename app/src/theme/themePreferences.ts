import { z } from "zod";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { ThemeName } from "@/theme/themes";

export const themePreferenceStorageKey = "class-manager.theme-preferences";

export interface ThemePreferenceStorage {
  read: () => Promise<string | null>;
  write: (serializedPreferences: string) => Promise<void>;
}

export interface ThemePreferences {
  themeName: ThemeName;
  colorSchemePreference: ColorSchemePreference;
}

const themePreferencesSchema = z.object({
  themeName: z.string(),
  colorSchemePreference: z.enum(["light", "dark", "system"]),
});

export function parseThemePreferences(
  serializedPreferences: string | null,
): ThemePreferences | null {
  if (serializedPreferences === null) {
    return null;
  }
  try {
    const parsed = themePreferencesSchema.safeParse(JSON.parse(serializedPreferences));
    return parsed.success ? parsed.data : null;
  } catch {
    return null;
  }
}

export function serializeThemePreferences(preferences: ThemePreferences): string {
  return JSON.stringify(preferences);
}

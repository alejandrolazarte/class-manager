import { useEffect, useRef } from "react";
import { themePreferenceStorage } from "@/theme/themePreferenceStorage";
import { parseThemePreferences, serializeThemePreferences } from "@/theme/themePreferences";
import { useTheme } from "@/theme/useTheme";

export function PersistedThemePreferences() {
  const { themeName, colorSchemePreference, setThemeName, setColorSchemePreference } = useTheme();
  const hasRestored = useRef(false);

  useEffect(() => {
    let isActive = true;
    themePreferenceStorage
      .read()
      .then((serializedPreferences) => {
        const preferences = parseThemePreferences(serializedPreferences);
        if (isActive && preferences) {
          setThemeName(preferences.themeName);
          setColorSchemePreference(preferences.colorSchemePreference);
        }
      })
      .catch(() => undefined)
      .finally(() => {
        hasRestored.current = true;
      });
    return () => {
      isActive = false;
    };
  }, [setThemeName, setColorSchemePreference]);

  useEffect(() => {
    if (!hasRestored.current) {
      return;
    }
    themePreferenceStorage
      .write(serializeThemePreferences({ themeName, colorSchemePreference }))
      .catch(() => undefined);
  }, [themeName, colorSchemePreference]);

  return null;
}

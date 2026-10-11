import { useEffect, useRef } from "react";
import { themePreferenceStorage } from "@/theme/themePreferenceStorage";
import { parseThemePreferences, serializeThemePreferences } from "@/theme/themePreferences";
import { useTheme } from "@/theme/useTheme";

export function PersistedThemePreferences() {
  const { colorSchemePreference, setColorSchemePreference } = useTheme();
  const hasRestored = useRef(false);

  useEffect(() => {
    let isActive = true;
    themePreferenceStorage
      .read()
      .then((serializedPreferences) => {
        const preferences = parseThemePreferences(serializedPreferences);
        if (isActive && preferences) {
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
  }, [setColorSchemePreference]);

  useEffect(() => {
    if (!hasRestored.current) {
      return;
    }
    themePreferenceStorage
      .write(serializeThemePreferences({ colorSchemePreference }))
      .catch(() => undefined);
  }, [colorSchemePreference]);

  return null;
}

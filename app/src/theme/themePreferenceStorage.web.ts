import { ThemePreferenceStorage, themePreferenceStorageKey } from "@/theme/themePreferences";

export const themePreferenceStorage: ThemePreferenceStorage = {
  read: async () => globalThis.localStorage?.getItem(themePreferenceStorageKey) ?? null,
  write: async (serializedPreferences) => {
    globalThis.localStorage?.setItem(themePreferenceStorageKey, serializedPreferences);
  },
};

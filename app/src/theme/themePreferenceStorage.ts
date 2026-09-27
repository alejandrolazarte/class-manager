import * as SecureStore from "expo-secure-store";
import { ThemePreferenceStorage, themePreferenceStorageKey } from "@/theme/themePreferences";

export const themePreferenceStorage: ThemePreferenceStorage = {
  read: () => SecureStore.getItemAsync(themePreferenceStorageKey),
  write: (serializedPreferences) =>
    SecureStore.setItemAsync(themePreferenceStorageKey, serializedPreferences),
};

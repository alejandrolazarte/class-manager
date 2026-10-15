import * as SecureStore from "expo-secure-store";
import { LanguagePreferenceStorage, languagePreferenceStorageKey } from "@/i18n/languagePreference";

export const languagePreferenceStorage: LanguagePreferenceStorage = {
  read: () => SecureStore.getItemAsync(languagePreferenceStorageKey),
  write: (language) => SecureStore.setItemAsync(languagePreferenceStorageKey, language),
};

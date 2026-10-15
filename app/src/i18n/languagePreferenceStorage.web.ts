import { LanguagePreferenceStorage, languagePreferenceStorageKey } from "@/i18n/languagePreference";

export const languagePreferenceStorage: LanguagePreferenceStorage = {
  read: async () => globalThis.localStorage?.getItem(languagePreferenceStorageKey) ?? null,
  write: async (language) => {
    globalThis.localStorage?.setItem(languagePreferenceStorageKey, language);
  },
};

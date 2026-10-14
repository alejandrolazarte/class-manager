import { defaultLanguage, isLanguage, Language } from "@/i18n/languages";

export const languagePreferenceStorageKey = "class-manager.language";

export interface LanguagePreferenceStorage {
  read: () => Promise<string | null>;
  write: (language: Language) => Promise<void>;
}

export function parseLanguagePreference(storedLanguage: string | null): Language {
  return isLanguage(storedLanguage) ? storedLanguage : defaultLanguage;
}

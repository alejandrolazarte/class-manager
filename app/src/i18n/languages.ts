import englishTranslations from "@/i18n/en.json";
import spanishTranslations from "@/i18n/es.json";

export const languages = ["es", "en"] as const;

export type Language = (typeof languages)[number];

export const defaultLanguage: Language = "es";

type Translations = Record<keyof typeof spanishTranslations, string>;

export const translationsByLanguage: Record<Language, Translations> = {
  es: spanishTranslations,
  en: englishTranslations,
};

export function isLanguage(value: string | null): value is Language {
  return languages.some((language) => language === value);
}

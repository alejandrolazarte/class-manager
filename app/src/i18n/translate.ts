import { defaultLanguage, Language, translationsByLanguage } from "@/i18n/languages";
import spanishTranslations from "@/i18n/es.json";

export type TranslationKey = keyof typeof spanishTranslations;

type TranslationValues = Record<string, string | number>;

const singularCount = 1;

export type PluralTranslationKey = {
  [Key in TranslationKey]: Key extends `${infer BaseKey}.one` ? BaseKey : never;
}[TranslationKey];

const placeholderPattern = /\{(\w+)\}/g;

let currentLanguage: Language = defaultLanguage;

export function setTranslationLanguage(language: Language): void {
  currentLanguage = language;
}

export function translate(key: TranslationKey, values: TranslationValues = {}): string {
  return translationsByLanguage[currentLanguage][key].replace(
    placeholderPattern,
    (placeholder, valueName: string) =>
      valueName in values ? String(values[valueName]) : placeholder,
  );
}

export function translateCount(
  key: PluralTranslationKey,
  count: number,
  values: TranslationValues = {},
): string {
  const form = count === singularCount ? "one" : "other";
  return translate(`${key}.${form}` as TranslationKey, { ...values, count });
}

export function hasTranslation(key: string): key is TranslationKey {
  return key in spanishTranslations;
}

import spanishTranslations from "@/i18n/es.json";

export type TranslationKey = keyof typeof spanishTranslations;

type TranslationValues = Record<string, string | number>;

const placeholderPattern = /\{(\w+)\}/g;

export function translate(key: TranslationKey, values: TranslationValues = {}): string {
  return spanishTranslations[key].replace(placeholderPattern, (placeholder, valueName: string) =>
    valueName in values ? String(values[valueName]) : placeholder,
  );
}

import { Language } from "@/i18n/languages";
import { translate, TranslationKey } from "@/i18n/translate";

const languageLabelKeys: Record<Language, TranslationKey> = {
  es: "settings.language.es",
  en: "settings.language.en",
};

export function languageValue(language: Language): string {
  return translate(languageLabelKeys[language]);
}

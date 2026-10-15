import { languages, translationsByLanguage } from "@/i18n/languages";

const placeholderPattern = /\{(\w+)\}/g;

function placeholdersOf(text: string): string[] {
  return [...text.matchAll(placeholderPattern)]
    .map(([, placeholderName]) => placeholderName)
    .sort();
}

describe("When a language is added", () => {
  it("Then it translates every key with the same placeholders", () => {
    const spanishTranslations = translationsByLanguage.es;

    const mismatches = languages.flatMap((language) => {
      const translations = translationsByLanguage[language];
      const extraKeys = Object.keys(translations).filter((key) => !(key in spanishTranslations));
      const keysWithOtherPlaceholders = Object.entries(spanishTranslations)
        .filter(
          ([key, spanishText]) =>
            placeholdersOf(translations[key as keyof typeof translations] ?? "").join() !==
            placeholdersOf(spanishText).join(),
        )
        .map(([key]) => key);
      return [...extraKeys, ...keysWithOtherPlaceholders].map((key) => `${language}: ${key}`);
    });

    expect(mismatches).toEqual([]);
  });
});

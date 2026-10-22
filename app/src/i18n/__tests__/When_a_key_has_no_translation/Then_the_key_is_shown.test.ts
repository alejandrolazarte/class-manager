import { translate, TranslationKey } from "@/i18n/translate";

const keyWithoutTranslation = "products.stock.kind.Unknown" as TranslationKey;

describe("When a key has no translation", () => {
  it("Then the key is shown", () => {
    expect(translate(keyWithoutTranslation)).toBe(keyWithoutTranslation);
  });
});

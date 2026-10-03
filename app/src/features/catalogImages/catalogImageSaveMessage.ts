import { CatalogImageSaveOutcome } from "@/features/catalogImages/catalogImageDraft";
import { TranslationKey } from "@/i18n/translate";

const outcomeMessageKeys: Record<Exclude<CatalogImageSaveOutcome, "saved">, TranslationKey> = {
  limitReached: "catalogImages.savedWithLimitReached",
  failed: "catalogImages.savedWithoutPhoto",
};

export function catalogImageSaveMessageKey(
  outcome: CatalogImageSaveOutcome,
  savedMessageKey: TranslationKey,
): TranslationKey {
  return outcome === "saved" ? savedMessageKey : outcomeMessageKeys[outcome];
}

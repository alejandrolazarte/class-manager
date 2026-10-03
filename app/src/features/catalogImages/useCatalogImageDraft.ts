import { useState } from "react";
import {
  CatalogImageDraft,
  previewUriOf,
  unchangedCatalogImage,
} from "@/features/catalogImages/catalogImageDraft";
import {
  catalogImageMaximumSizeInBytes,
  pickCatalogImage,
} from "@/features/catalogImages/pickCatalogImage";
import { translate } from "@/i18n/translate";

export interface CatalogImageDraftState {
  draft: CatalogImageDraft;
  previewUri: string | null;
  errorMessage: string | null;
  pick: () => Promise<void>;
  remove: () => void;
}

export function useCatalogImageDraft(currentImageUrl: string | null): CatalogImageDraftState {
  const [draft, setDraft] = useState<CatalogImageDraft>(unchangedCatalogImage);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const pick = async () => {
    setErrorMessage(null);
    const file = await pickCatalogImage();
    if (file === null) {
      return;
    }
    if (file.size !== undefined && file.size > catalogImageMaximumSizeInBytes) {
      setErrorMessage(translate("catalogImages.tooLarge"));
      return;
    }
    setDraft({ kind: "replaced", file });
  };

  const remove = () => {
    setErrorMessage(null);
    setDraft({ kind: "removed" });
  };

  return { draft, previewUri: previewUriOf(draft, currentImageUrl), errorMessage, pick, remove };
}

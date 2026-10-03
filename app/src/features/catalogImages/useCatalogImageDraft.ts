import { useRef, useState } from "react";
import { CatalogImageDraft, draftFrom } from "@/features/catalogImages/catalogImageDraft";
import {
  catalogImageMaximumSizeInBytes,
  pickCatalogImage,
} from "@/features/catalogImages/pickCatalogImage";
import { CatalogImage } from "@/features/catalogImages/types";
import { translate } from "@/i18n/translate";

export interface CatalogImageDraftState {
  draft: CatalogImageDraft;
  errorMessage: string | null;
  pick: () => Promise<void>;
  remove: (key: string) => void;
  makeMain: (key: string) => void;
}

const newImageKeyPrefix = "new-";

export function useCatalogImageDraft(savedImages: CatalogImage[]): CatalogImageDraftState {
  const [draft, setDraft] = useState<CatalogImageDraft>(() => draftFrom(savedImages));
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const nextNewImageNumber = useRef(0);

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
    nextNewImageNumber.current += 1;
    const key = `${newImageKeyPrefix}${nextNewImageNumber.current}`;
    setDraft((current) => ({ ...current, items: [...current.items, { kind: "new", key, file }] }));
  };

  const remove = (key: string) => {
    setErrorMessage(null);
    setDraft((current) => {
      const removedItem = current.items.find((item) => item.key === key);
      return {
        items: current.items.filter((item) => item.key !== key),
        removedIds:
          removedItem?.kind === "stored"
            ? [...current.removedIds, removedItem.image.id]
            : current.removedIds,
      };
    });
  };

  const makeMain = (key: string) => {
    setDraft((current) => {
      const mainItem = current.items.find((item) => item.key === key);
      return mainItem === undefined
        ? current
        : { ...current, items: [mainItem, ...current.items.filter((item) => item.key !== key)] };
    });
  };

  return { draft, errorMessage, pick, remove, makeMain };
}

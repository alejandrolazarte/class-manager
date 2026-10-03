import { PickedFile } from "@/api/fileForm";
import { isApiError } from "@/api/httpClient";
import { CatalogImage, CatalogImageOwner } from "@/features/catalogImages/types";

export type CatalogImageDraftItem =
  | { kind: "stored"; key: string; image: CatalogImage }
  | { kind: "new"; key: string; file: PickedFile };

export interface CatalogImageDraft {
  items: CatalogImageDraftItem[];
  removedIds: string[];
}

export type CatalogImageSaveOutcome = "saved" | "limitReached" | "failed";

export interface CatalogImageActions {
  add: (ownerId: string, file: PickedFile) => Promise<CatalogImageOwner>;
  remove: (ownerId: string, imageId: string) => Promise<CatalogImageOwner>;
  reorder: (ownerId: string, imageIds: string[]) => Promise<CatalogImageOwner>;
}

export const featureLimitReachedCode = "feature.limit_reached";

export function draftFrom(images: CatalogImage[]): CatalogImageDraft {
  return {
    items: images.map((image) => ({ kind: "stored", key: image.id, image })),
    removedIds: [],
  };
}

export function previewUriOf(item: CatalogImageDraftItem): string {
  return item.kind === "stored" ? item.image.url : item.file.uri;
}

function imageIdsOf(owner: CatalogImageOwner): string[] {
  return owner.images.map((image) => image.id);
}

function haveSameOrder(firstIds: string[], secondIds: string[]): boolean {
  return (
    firstIds.length === secondIds.length &&
    firstIds.every((imageId, index) => imageId === secondIds[index])
  );
}

export async function applyCatalogImageDraft(
  draft: CatalogImageDraft,
  ownerId: string,
  savedImages: CatalogImage[],
  actions: CatalogImageActions,
): Promise<CatalogImageSaveOutcome> {
  try {
    let currentIds = savedImages.map((image) => image.id);
    for (const removedId of draft.removedIds) {
      currentIds = imageIdsOf(await actions.remove(ownerId, removedId));
    }
    const addedIdsByKey = new Map<string, string>();
    for (const item of draft.items) {
      if (item.kind === "new") {
        const knownIds = currentIds;
        currentIds = imageIdsOf(await actions.add(ownerId, item.file));
        const addedId = currentIds.find((imageId) => !knownIds.includes(imageId));
        if (addedId !== undefined) {
          addedIdsByKey.set(item.key, addedId);
        }
      }
    }
    const wantedIds = draft.items
      .map((item) => (item.kind === "stored" ? item.image.id : addedIdsByKey.get(item.key)))
      .filter((imageId): imageId is string => imageId !== undefined);
    if (!haveSameOrder(wantedIds, currentIds)) {
      await actions.reorder(ownerId, wantedIds);
    }
    return "saved";
  } catch (saveError) {
    return isApiError(saveError) && saveError.hasCode(featureLimitReachedCode)
      ? "limitReached"
      : "failed";
  }
}

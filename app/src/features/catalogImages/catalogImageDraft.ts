import { PickedFile } from "@/api/fileForm";

export type CatalogImageDraft =
  { kind: "unchanged" } | { kind: "replaced"; file: PickedFile } | { kind: "removed" };

export interface CatalogImageActions {
  upload: (ownerId: string, file: PickedFile) => Promise<unknown>;
  remove: (ownerId: string) => Promise<unknown>;
}

export const unchangedCatalogImage: CatalogImageDraft = { kind: "unchanged" };

export function previewUriOf(
  draft: CatalogImageDraft,
  currentImageUrl: string | null,
): string | null {
  if (draft.kind === "replaced") {
    return draft.file.uri;
  }
  return draft.kind === "removed" ? null : currentImageUrl;
}

export async function applyCatalogImageDraft(
  draft: CatalogImageDraft,
  ownerId: string,
  hadImage: boolean,
  actions: CatalogImageActions,
): Promise<void> {
  if (draft.kind === "replaced") {
    await actions.upload(ownerId, draft.file);
  } else if (draft.kind === "removed" && hadImage) {
    await actions.remove(ownerId);
  }
}

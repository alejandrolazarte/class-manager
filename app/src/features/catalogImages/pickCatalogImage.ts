import { getDocumentAsync } from "expo-document-picker";
import { PickedFile } from "@/api/fileForm";

export const catalogImageMaximumSizeInBytes = 5 * 1024 * 1024;

const catalogImageTypes = ["image/png", "image/jpeg", "image/webp"];

export async function pickCatalogImage(): Promise<PickedFile | null> {
  const result = await getDocumentAsync({ type: catalogImageTypes, copyToCacheDirectory: true });
  const asset = result.canceled ? undefined : result.assets[0];
  return asset === undefined
    ? null
    : {
        name: asset.name,
        uri: asset.uri,
        mimeType: asset.mimeType,
        size: asset.size,
        webFile: asset.file,
      };
}

import { getDocumentAsync } from "expo-document-picker";
import { PickedFile } from "@/api/fileForm";

export const userPhotoMaximumSizeInBytes = 10 * 1024 * 1024;

const userPhotoTypes = ["image/png", "image/jpeg", "image/webp"];

export async function pickUserPhoto(): Promise<PickedFile | null> {
  const result = await getDocumentAsync({ type: userPhotoTypes, copyToCacheDirectory: true });
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

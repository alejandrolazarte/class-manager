import { getDocumentAsync } from "expo-document-picker";
import { PickedFile } from "@/api/fileForm";

export type PickedLogoFile = PickedFile;

export const logoMaximumSizeInBytes = 512 * 1024;

const logoDocumentTypes = ["image/png", "image/jpeg", "image/webp"];

export async function pickLogoFile(): Promise<PickedLogoFile | null> {
  const result = await getDocumentAsync({ type: logoDocumentTypes, copyToCacheDirectory: true });
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

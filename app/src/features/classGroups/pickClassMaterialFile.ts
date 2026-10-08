import { getDocumentAsync } from "expo-document-picker";
import { PickedFile } from "@/api/fileForm";

export const classMaterialMaximumSizeInBytes = 20 * 1024 * 1024;

const pdfMimeType = "application/pdf";

export async function pickClassMaterialFile(): Promise<PickedFile | null> {
  const result = await getDocumentAsync({ type: pdfMimeType, copyToCacheDirectory: true });
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

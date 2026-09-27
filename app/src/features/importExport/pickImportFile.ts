import { getDocumentAsync } from "expo-document-picker";
import { xlsxMimeType } from "@/features/importExport/spreadsheetFile";

export interface PickedImportFile {
  name: string;
  uri: string;
  mimeType?: string;
  webFile?: File;
}

const importDocumentTypes = [
  xlsxMimeType,
  "text/csv",
  "text/comma-separated-values",
  "text/plain",
  "application/csv",
  "application/vnd.ms-excel",
];

export async function pickImportFile(): Promise<PickedImportFile | null> {
  const result = await getDocumentAsync({ type: importDocumentTypes, copyToCacheDirectory: true });
  const asset = result.canceled ? undefined : result.assets[0];
  return asset === undefined
    ? null
    : { name: asset.name, uri: asset.uri, mimeType: asset.mimeType, webFile: asset.file };
}

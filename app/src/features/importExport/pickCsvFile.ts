import { getDocumentAsync } from "expo-document-picker";

export interface PickedCsvFile {
  name: string;
  uri: string;
  webFile?: File;
}

const csvDocumentTypes = [
  "text/csv",
  "text/comma-separated-values",
  "text/plain",
  "application/csv",
  "application/vnd.ms-excel",
];

export async function pickCsvFile(): Promise<PickedCsvFile | null> {
  const result = await getDocumentAsync({ type: csvDocumentTypes, copyToCacheDirectory: true });
  const asset = result.canceled ? undefined : result.assets[0];
  return asset === undefined ? null : { name: asset.name, uri: asset.uri, webFile: asset.file };
}

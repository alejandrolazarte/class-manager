import { File, Paths } from "expo-file-system";
import { shareAsync } from "expo-sharing";
import { xlsxMimeType, xlsxUniformTypeIdentifier } from "@/features/importExport/spreadsheetFile";

export async function saveSpreadsheetFile(fileName: string, content: ArrayBuffer): Promise<void> {
  const file = new File(Paths.cache, fileName);
  if (file.exists) {
    file.delete();
  }
  file.create();
  file.write(new Uint8Array(content));
  await shareAsync(file.uri, {
    mimeType: xlsxMimeType,
    UTI: xlsxUniformTypeIdentifier,
    dialogTitle: fileName,
  });
}

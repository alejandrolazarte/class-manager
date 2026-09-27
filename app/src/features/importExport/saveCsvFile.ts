import { File, Paths } from "expo-file-system";
import { shareAsync } from "expo-sharing";
import { csvMimeType, withByteOrderMark } from "@/features/importExport/csvText";

const csvUniformTypeIdentifier = "public.comma-separated-values-text";

export async function saveCsvFile(fileName: string, csvText: string): Promise<void> {
  const file = new File(Paths.cache, fileName);
  if (file.exists) {
    file.delete();
  }
  file.create();
  file.write(withByteOrderMark(csvText));
  await shareAsync(file.uri, {
    mimeType: csvMimeType,
    UTI: csvUniformTypeIdentifier,
    dialogTitle: fileName,
  });
}

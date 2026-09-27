import { csvMimeType, withByteOrderMark } from "@/features/importExport/csvText";

const csvContentType = `${csvMimeType};charset=utf-8`;

export async function saveCsvFile(fileName: string, csvText: string): Promise<void> {
  const blob = new Blob([withByteOrderMark(csvText)], { type: csvContentType });
  const objectUrl = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = objectUrl;
  link.download = fileName;
  link.click();
  URL.revokeObjectURL(objectUrl);
}

import { xlsxMimeType } from "@/features/importExport/spreadsheetFile";

export async function saveSpreadsheetFile(fileName: string, content: ArrayBuffer): Promise<void> {
  const blob = new Blob([content], { type: xlsxMimeType });
  const objectUrl = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = objectUrl;
  link.download = fileName;
  link.click();
  URL.revokeObjectURL(objectUrl);
}

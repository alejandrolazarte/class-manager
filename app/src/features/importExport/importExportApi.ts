import { httpClient } from "@/api/httpClient";
import { csvFileForm } from "@/features/importExport/csvFileForm";
import { PickedCsvFile } from "@/features/importExport/pickCsvFile";
import { ImportModule, ImportReport, ImportSchema } from "@/features/importExport/types";

const importExportPath = "/api/import-export";

function modulePath(module: ImportModule, action: string): string {
  return `${importExportPath}/${encodeURIComponent(module)}/${action}`;
}

export function getImportSchema(module: ImportModule): Promise<ImportSchema> {
  return httpClient.get<ImportSchema>(modulePath(module, "schema"));
}

export function previewImport(module: ImportModule, file: PickedCsvFile): Promise<ImportReport> {
  return httpClient.postForm<ImportReport>(modulePath(module, "preview"), csvFileForm(file));
}

export function importFile(module: ImportModule, file: PickedCsvFile): Promise<ImportReport> {
  return httpClient.postForm<ImportReport>(modulePath(module, "import"), csvFileForm(file));
}

export function downloadTemplate(module: ImportModule): Promise<string> {
  return httpClient.getText(modulePath(module, "template"));
}

export function downloadExport(module: ImportModule): Promise<string> {
  return httpClient.getText(modulePath(module, "export"));
}

import { httpClient } from "@/api/httpClient";
import { importFileForm } from "@/features/importExport/importFileForm";
import { PickedImportFile } from "@/features/importExport/pickImportFile";
import { ImportModule, ImportReport, ImportSchema } from "@/features/importExport/types";

const importExportPath = "/api/import-export";

function modulePath(module: ImportModule, action: string): string {
  return `${importExportPath}/${encodeURIComponent(module)}/${action}`;
}

export function getImportSchema(module: ImportModule): Promise<ImportSchema> {
  return httpClient.get<ImportSchema>(modulePath(module, "schema"));
}

export function previewImport(module: ImportModule, file: PickedImportFile): Promise<ImportReport> {
  return httpClient.postForm<ImportReport>(modulePath(module, "preview"), importFileForm(file));
}

export function importFile(module: ImportModule, file: PickedImportFile): Promise<ImportReport> {
  return httpClient.postForm<ImportReport>(modulePath(module, "import"), importFileForm(file));
}

export function downloadTemplate(module: ImportModule): Promise<ArrayBuffer> {
  return httpClient.getBytes(modulePath(module, "template"));
}

export function downloadExport(module: ImportModule): Promise<ArrayBuffer> {
  return httpClient.getBytes(modulePath(module, "export"));
}

import { PickedImportFile } from "@/features/importExport/pickImportFile";
import { genericFileMimeType } from "@/features/importExport/spreadsheetFile";

const fileFieldName = "file";

interface NativeFormFile {
  uri: string;
  name: string;
  type: string;
}

export function importFileForm(file: PickedImportFile): FormData {
  const form = new FormData();
  if (file.webFile !== undefined) {
    form.append(fileFieldName, file.webFile, file.name);
  } else {
    const nativeFile: NativeFormFile = {
      uri: file.uri,
      name: file.name,
      type: file.mimeType ?? genericFileMimeType,
    };
    form.append(fileFieldName, nativeFile as unknown as Blob);
  }
  return form;
}

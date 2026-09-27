import { PickedCsvFile } from "@/features/importExport/pickCsvFile";

const fileFieldName = "file";
const csvMimeType = "text/csv";

interface NativeFormFile {
  uri: string;
  name: string;
  type: string;
}

export function csvFileForm(file: PickedCsvFile): FormData {
  const form = new FormData();
  if (file.webFile !== undefined) {
    form.append(fileFieldName, file.webFile, file.name);
  } else {
    const nativeFile: NativeFormFile = { uri: file.uri, name: file.name, type: csvMimeType };
    form.append(fileFieldName, nativeFile as unknown as Blob);
  }
  return form;
}

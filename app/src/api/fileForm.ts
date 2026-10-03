export interface PickedFile {
  name: string;
  uri: string;
  mimeType?: string;
  size?: number;
  webFile?: File;
}

interface NativeFormFile {
  uri: string;
  name: string;
  type: string;
}

const fileFieldName = "file";
const genericBinaryMimeType = "application/octet-stream";

export function toFileForm(file: PickedFile): FormData {
  const form = new FormData();
  if (file.webFile !== undefined) {
    form.append(fileFieldName, file.webFile, file.name);
  } else {
    const nativeFile: NativeFormFile = {
      uri: file.uri,
      name: file.name,
      type: file.mimeType ?? genericBinaryMimeType,
    };
    form.append(fileFieldName, nativeFile as unknown as Blob);
  }
  return form;
}

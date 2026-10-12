import { translate } from "@/i18n/translate";

const bytesPerMegabyte = 1024 * 1024;

export function materialFileSizeLabel(sizeInBytes: number): string {
  return translate("classGroups.form.materialFileSize", {
    megabytes: (sizeInBytes / bytesPerMegabyte).toFixed(1).replace(".", ","),
  });
}

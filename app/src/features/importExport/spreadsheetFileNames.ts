import { format } from "date-fns";
import { ImportModule } from "@/features/importExport/types";
import { translate, TranslationKey } from "@/i18n/translate";

const fileDateFormat = "yyyy-MM-dd";

const fileModuleKeys: Record<ImportModule, TranslationKey> = {
  students: "importExport.fileModule.students",
  instructors: "importExport.fileModule.instructors",
};

export function fileModuleName(module: ImportModule): string {
  return translate(fileModuleKeys[module]);
}

export const spreadsheetFileNames = {
  template: (module: ImportModule) =>
    translate("importExport.fileName.template", { module: fileModuleName(module) }),
  export: (module: ImportModule, date: Date) =>
    translate("importExport.fileName.export", {
      module: fileModuleName(module),
      date: format(date, fileDateFormat),
    }),
};

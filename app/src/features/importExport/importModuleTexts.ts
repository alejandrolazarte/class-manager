import { ImportModule } from "@/features/importExport/types";
import { TranslationKey } from "@/i18n/translate";

interface ImportModuleTexts {
  importTitle: TranslationKey;
  exportLabel: TranslationKey;
  subtitle: TranslationKey;
  fillStep: TranslationKey;
  viewList: TranslationKey;
}

export const importModuleTexts: Record<ImportModule, ImportModuleTexts> = {
  students: {
    importTitle: "importExport.import.students",
    exportLabel: "importExport.export.students",
    subtitle: "importExport.subtitle.students",
    fillStep: "importExport.steps.fill.students",
    viewList: "importExport.result.viewStudents",
  },
  instructors: {
    importTitle: "importExport.import.instructors",
    exportLabel: "importExport.export.instructors",
    subtitle: "importExport.subtitle.instructors",
    fillStep: "importExport.steps.fill.instructors",
    viewList: "importExport.result.viewInstructors",
  },
};

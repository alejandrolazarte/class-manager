import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import {
  downloadExport,
  downloadTemplate,
  getImportSchema,
  importFile,
  previewImport,
} from "@/features/importExport/importExportApi";
import { csvFileNames } from "@/features/importExport/csvFileNames";
import { importExportQueryKeys } from "@/features/importExport/importExportQueryKeys";
import { PickedCsvFile } from "@/features/importExport/pickCsvFile";
import { saveCsvFile } from "@/features/importExport/saveCsvFile";
import { ImportModule } from "@/features/importExport/types";
import { instructorQueryKeys } from "@/features/instructors/instructorQueryKeys";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";

interface ImportFileVariables {
  module: ImportModule;
  file: PickedCsvFile;
}

export function useImportSchema(module: ImportModule) {
  return useQuery({
    queryKey: importExportQueryKeys.schema(module),
    queryFn: () => getImportSchema(module),
  });
}

export function usePreviewImport() {
  return useMutation({
    mutationFn: ({ module, file }: ImportFileVariables) => previewImport(module, file),
  });
}

export function useImportFile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ module, file }: ImportFileVariables) => importFile(module, file),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: instructorQueryKeys.all }),
      ]),
  });
}

export function useDownloadTemplate() {
  return useMutation({
    mutationFn: async (module: ImportModule) =>
      saveCsvFile(csvFileNames.template(module), await downloadTemplate(module)),
  });
}

export function useDownloadExport() {
  return useMutation({
    mutationFn: async (module: ImportModule) =>
      saveCsvFile(csvFileNames.export(module, new Date()), await downloadExport(module)),
  });
}

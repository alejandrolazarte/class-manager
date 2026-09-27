import { ImportModule } from "@/features/importExport/types";

export const importExportQueryKeys = {
  all: ["importExport"] as const,
  schema: (module: ImportModule) => [...importExportQueryKeys.all, "schema", module] as const,
};

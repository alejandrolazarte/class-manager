import { isApiError, isNetworkError } from "@/api/httpClient";
import { ImportCellError, MappedHeader } from "@/features/importExport/types";
import { hasTranslation, translate } from "@/i18n/translate";

const rowErrorKeyPrefix = "importExport.rowError.";
const fileErrorKeyPrefix = "importExport.fileError.";

function columnHeader(key: string, headers: MappedHeader[]): string {
  return headers.find((header) => header.key === key)?.header ?? key;
}

export function rowErrorMessage(error: ImportCellError, headers: MappedHeader[]): string {
  const translationKey = `${rowErrorKeyPrefix}${error.code}`;
  const column = columnHeader(error.key, headers);
  return hasTranslation(translationKey)
    ? translate(translationKey, { column })
    : translate("importExport.rowError.invalid", { column });
}

export function fileErrorMessage(requestError: unknown): string {
  if (isNetworkError(requestError)) {
    return translate("common.networkError");
  }
  const translationKey = isApiError(requestError)
    ? `${fileErrorKeyPrefix}${requestError.problem.code ?? ""}`
    : "";
  return hasTranslation(translationKey)
    ? translate(translationKey)
    : translate("common.unexpectedError");
}

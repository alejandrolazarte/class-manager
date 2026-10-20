import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { hasTranslation, translate } from "@/i18n/translate";

export function operatorErrorMessage(error: unknown): string {
  const codeKey =
    error instanceof BackofficeRequestError ? `apiErrors.${error.problem.code ?? ""}` : "";
  return hasTranslation(codeKey) ? translate(codeKey) : translate("common.unexpectedError");
}

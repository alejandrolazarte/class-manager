import { FieldValues, Path, UseFormReturn } from "react-hook-form";
import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { hasTranslation, translate } from "@/i18n/translate";

export function applySubmissionError<TValues extends FieldValues>(
  form: UseFormReturn<TValues>,
  error: unknown,
  fieldNames: readonly Path<TValues>[],
): string | null {
  if (!(error instanceof BackofficeRequestError)) {
    return translate("common.unexpectedError");
  }
  if (applyServerFieldErrors(form, error.problem, fieldNames)) {
    return null;
  }
  const codeKey = `apiErrors.${error.problem.code ?? ""}`;
  return hasTranslation(codeKey) ? translate(codeKey) : translate("common.unexpectedError");
}

export function amountText(amount: number | null): string {
  return amount === null ? "" : String(amount).replace(".", ",");
}

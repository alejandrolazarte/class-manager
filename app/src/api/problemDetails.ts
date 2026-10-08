import { hasTranslation, translate } from "@/i18n/translate";

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  code?: string;
  errors?: Record<string, string[]>;
  [extension: string]: unknown;
}

export type FieldErrors = Record<string, string>;

export function toCamelCase(fieldName: string): string {
  return fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
}

function localizedFieldError(problem: ProblemDetails, fieldName: string): string {
  const codeKey = `apiErrors.${problem.code ?? ""}`;
  if (hasTranslation(codeKey)) {
    return translate(codeKey);
  }
  const fieldKey = `apiErrors.field.${fieldName}`;
  return hasTranslation(fieldKey) ? translate(fieldKey) : translate("apiErrors.invalidValue");
}

export function getFieldErrors(problem: ProblemDetails): FieldErrors {
  const fieldErrors: FieldErrors = {};
  for (const [fieldName, messages] of Object.entries(problem.errors ?? {})) {
    if (messages.length > 0) {
      const camelCaseFieldName = toCamelCase(fieldName);
      fieldErrors[camelCaseFieldName] = localizedFieldError(problem, camelCaseFieldName);
    }
  }
  return fieldErrors;
}

export function getStringExtension(
  problem: ProblemDetails,
  extensionName: string,
): string | undefined {
  const extensionValue = problem[extensionName];
  return typeof extensionValue === "string" ? extensionValue : undefined;
}

export function getNumberExtension(
  problem: ProblemDetails,
  extensionName: string,
): number | undefined {
  const extensionValue = problem[extensionName];
  return typeof extensionValue === "number" ? extensionValue : undefined;
}

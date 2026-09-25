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

export function getFieldErrors(problem: ProblemDetails): FieldErrors {
  const fieldErrors: FieldErrors = {};
  for (const [fieldName, messages] of Object.entries(problem.errors ?? {})) {
    const firstMessage = messages[0];
    if (firstMessage !== undefined) {
      fieldErrors[toCamelCase(fieldName)] = firstMessage;
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

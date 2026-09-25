import { FieldValues, Path, UseFormReturn } from "react-hook-form";
import { getFieldErrors, ProblemDetails } from "@/api/problemDetails";

export function applyServerFieldErrors<TFormValues extends FieldValues>(
  form: UseFormReturn<TFormValues>,
  problem: ProblemDetails,
  formFieldNames: readonly Path<TFormValues>[],
): boolean {
  const knownFieldErrors = Object.entries(getFieldErrors(problem)).filter(([fieldName]) =>
    (formFieldNames as readonly string[]).includes(fieldName),
  );
  knownFieldErrors.forEach(([fieldName, message]) => {
    form.setError(fieldName as Path<TFormValues>, { type: "server", message });
  });
  return knownFieldErrors.length > 0;
}

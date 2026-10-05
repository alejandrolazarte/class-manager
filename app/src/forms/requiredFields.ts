import { Control, FieldValues, Path, useWatch } from "react-hook-form";

export function isFilled(value: unknown): boolean {
  if (typeof value === "string") {
    return value.trim().length > 0;
  }
  if (Array.isArray(value)) {
    return value.length > 0;
  }
  return value !== null && value !== undefined;
}

export function areFilled(values: readonly unknown[]): boolean {
  return values.every(isFilled);
}

export function useRequiredFieldsFilled<TFieldValues extends FieldValues>(
  control: Control<TFieldValues>,
  requiredFieldNames: readonly Path<TFieldValues>[],
): boolean {
  const values: unknown[] = useWatch({ control, name: [...requiredFieldNames] });
  return areFilled(values);
}

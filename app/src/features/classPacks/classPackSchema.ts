import { z } from "zod";
import { ClassPack, SaveClassPackRequest } from "@/features/classPacks/types";
import { parseAmount } from "@/features/fees/money";
import { amountSchema } from "@/features/fees/paymentSchema";
import { translate } from "@/i18n/translate";

export const classPackLimits = {
  nameMinimumLength: 2,
  nameMaximumLength: 60,
  minimumClassCount: 1,
  maximumClassCount: 100,
  minimumValidityMonths: 1,
  maximumValidityMonths: 24,
} as const;

const wholeNumberPattern = /^\d+$/;

function isWholeNumberBetween(text: string, minimum: number, maximum: number): boolean {
  const trimmedText = text.trim();
  if (!wholeNumberPattern.test(trimmedText)) {
    return false;
  }
  const value = Number(trimmedText);
  return value >= minimum && value <= maximum;
}

export const classPackSchema = z.object({
  name: z
    .string()
    .trim()
    .min(classPackLimits.nameMinimumLength, translate("classPacks.validation.nameInvalid"))
    .max(classPackLimits.nameMaximumLength, translate("classPacks.validation.nameInvalid")),
  classCount: z
    .string()
    .refine(
      (classCount) =>
        isWholeNumberBetween(
          classCount,
          classPackLimits.minimumClassCount,
          classPackLimits.maximumClassCount,
        ),
      translate("classPacks.validation.classCountInvalid"),
    ),
  price: amountSchema,
  validityMonths: z
    .string()
    .refine(
      (validityMonths) =>
        validityMonths.trim().length === 0 ||
        isWholeNumberBetween(
          validityMonths,
          classPackLimits.minimumValidityMonths,
          classPackLimits.maximumValidityMonths,
        ),
      translate("classPacks.validation.validityInvalid"),
    ),
});

export type ClassPackFormValues = z.input<typeof classPackSchema>;

export const classPackFieldNames = [
  "name",
  "classCount",
  "price",
  "validityMonths",
] as const satisfies readonly (keyof ClassPackFormValues)[];

export function toClassPackFormValues(classPack?: ClassPack): ClassPackFormValues {
  return {
    name: classPack?.name ?? "",
    classCount: classPack ? String(classPack.classCount) : "",
    price: classPack ? String(classPack.price).replace(".", ",") : "",
    validityMonths: classPack?.validityMonths ? String(classPack.validityMonths) : "",
  };
}

export function toSaveClassPackRequest(formValues: ClassPackFormValues): SaveClassPackRequest {
  const validityMonths = formValues.validityMonths.trim();
  return {
    name: formValues.name.trim(),
    classCount: Number(formValues.classCount.trim()),
    price: parseAmount(formValues.price) ?? 0,
    validityMonths: validityMonths.length === 0 ? null : Number(validityMonths),
  };
}

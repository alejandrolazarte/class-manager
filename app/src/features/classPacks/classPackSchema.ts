import { z } from "zod";
import { ClassPack, SaveClassPackRequest } from "@/features/classPacks/types";
import { parseAmount } from "@/formatting/money";
import { amountSchema } from "@/features/fees/paymentSchema";
import { translate } from "@/i18n/translate";

export const classPackLimits = {
  nameMinimumLength: 2,
  nameMaximumLength: 60,
  minimumClassCount: 1,
  maximumClassCount: 100,
  minimumValidityMonths: 1,
  maximumValidityMonths: 24,
  descriptionMaximumLength: 500,
  maximumImageCount: 1,
} as const;

export const classPackKinds = ["groupClasses", "privateLessons"] as const;

export type ClassPackKind = (typeof classPackKinds)[number];

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
  description: z
    .string()
    .trim()
    .max(
      classPackLimits.descriptionMaximumLength,
      translate("classPacks.validation.descriptionTooLong"),
    ),
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
  kind: z.enum(classPackKinds),
  classDurationMinutes: z.string(),
  classGroupIds: z.array(z.string()),
  coversAnyGroupClass: z.boolean(),
});

export type ClassPackFormValues = z.input<typeof classPackSchema>;

export const classPackFieldNames = [
  "name",
  "description",
  "classCount",
  "price",
  "validityMonths",
  "classDurationMinutes",
  "classGroupIds",
] as const satisfies readonly (keyof ClassPackFormValues)[];

export function toClassPackFormValues(classPack?: ClassPack): ClassPackFormValues {
  return {
    name: classPack?.name ?? "",
    description: classPack?.description ?? "",
    classCount: classPack ? String(classPack.classCount) : "",
    price: classPack ? String(classPack.price).replace(".", ",") : "",
    validityMonths: classPack?.validityMonths ? String(classPack.validityMonths) : "",
    kind:
      classPack === undefined || classPack.classGroupIds.length > 0 || classPack.coversAnyGroupClass
        ? "groupClasses"
        : "privateLessons",
    classDurationMinutes: classPack?.classDurationMinutes
      ? String(classPack.classDurationMinutes)
      : "",
    classGroupIds: classPack?.classGroupIds ?? [],
    coversAnyGroupClass: classPack?.coversAnyGroupClass ?? false,
  };
}

const baseRequiredClassPackFields = ["name", "classCount", "price"] as const;

export function requiredClassPackFields(kind: ClassPackKind, coversAnyGroupClass: boolean) {
  if (kind === "privateLessons") {
    return [...baseRequiredClassPackFields, "classDurationMinutes"] as const;
  }
  return coversAnyGroupClass
    ? baseRequiredClassPackFields
    : ([...baseRequiredClassPackFields, "classGroupIds"] as const);
}

export function toSaveClassPackRequest(formValues: ClassPackFormValues): SaveClassPackRequest {
  const validityMonths = formValues.validityMonths.trim();
  const description = formValues.description.trim();
  const isForGroupClasses = formValues.kind === "groupClasses";
  const coversAnyGroupClass = isForGroupClasses && formValues.coversAnyGroupClass;
  return {
    name: formValues.name.trim(),
    description: description.length === 0 ? null : description,
    classCount: Number(formValues.classCount.trim()),
    price: parseAmount(formValues.price) ?? 0,
    validityMonths: validityMonths.length === 0 ? null : Number(validityMonths),
    classDurationMinutes: isForGroupClasses ? null : Number(formValues.classDurationMinutes),
    classGroupIds: isForGroupClasses && !coversAnyGroupClass ? formValues.classGroupIds : [],
    coversAnyGroupClass,
  };
}

import { z } from "zod";
import { parseBirthDate } from "@/dates/birthDateFormatting";
import { todayIsoDate } from "@/dates/dates";
import { parseAmount } from "@/formatting/money";
import { translate } from "@/i18n/translate";

const currencyPattern = /^[A-Za-z]{3}$/;
const wholeNumberPattern = /^\d+$/;
const noteMaximumLength = 200;

function isEmpty(text: string): boolean {
  return text.trim() === "";
}

const termsShape = {
  price: z
    .string()
    .refine(
      (typedPrice) => parseAmount(typedPrice) !== null,
      translate("backoffice.validation.price"),
    ),
  currency: z
    .string()
    .refine(
      (typedCurrency) => currencyPattern.test(typedCurrency.trim()),
      translate("backoffice.validation.currency"),
    ),
  expiredOn: z.string().refine((typedDate) => {
    if (isEmpty(typedDate)) {
      return true;
    }
    const isoDate = parseBirthDate(typedDate);
    return isoDate !== null && isoDate >= todayIsoDate();
  }, translate("backoffice.validation.expiredOn")),
};

export const changePlanSchema = z.object({
  planCode: z.string().min(1),
  note: z.string().max(noteMaximumLength, translate("backoffice.validation.note")),
  ...termsShape,
});

export const giveAddOnSchema = z.object({
  featureCode: z.string().min(1),
  limit: z
    .string()
    .refine(
      (typedLimit) => isEmpty(typedLimit) || wholeNumberPattern.test(typedLimit.trim()),
      translate("backoffice.validation.limit"),
    ),
  ...termsShape,
});

export type ChangePlanFormValues = z.input<typeof changePlanSchema>;

export type GiveAddOnFormValues = z.input<typeof giveAddOnSchema>;

function optionalIsoDate(typedDate: string): string | null {
  return isEmpty(typedDate) ? null : parseBirthDate(typedDate);
}

export function toChangePlanRequest(values: ChangePlanFormValues) {
  const note = values.note.trim();
  return {
    planCode: values.planCode,
    price: parseAmount(values.price) ?? 0,
    currency: values.currency.trim().toUpperCase(),
    note: note === "" ? null : note,
    expiredOn: optionalIsoDate(values.expiredOn),
  };
}

export function toGiveAddOnRequest(values: GiveAddOnFormValues) {
  return {
    featureCode: values.featureCode,
    price: parseAmount(values.price) ?? 0,
    currency: values.currency.trim().toUpperCase(),
    limit: isEmpty(values.limit) ? null : Number(values.limit.trim()),
    expiredOn: optionalIsoDate(values.expiredOn),
  };
}

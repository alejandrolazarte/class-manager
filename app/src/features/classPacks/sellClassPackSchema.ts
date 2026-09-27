import { z } from "zod";
import { SellClassPackRequest } from "@/features/classPacks/types";
import { parseAmount } from "@/features/fees/money";
import { amountSchema, paymentMethods } from "@/features/fees/paymentSchema";
import { PaymentMethod } from "@/features/fees/types";
import { isValidBirthDate, parseBirthDate } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";

const notesMaximumLength = 200;

export const sellClassPackSchema = z.object({
  classPackId: z.string().min(1),
  price: amountSchema,
  method: z.enum(paymentMethods as [PaymentMethod, ...PaymentMethod[]]),
  purchasedOn: z
    .string()
    .refine(
      (purchasedOn) => isValidBirthDate(purchasedOn),
      translate("fees.validation.paidOnInvalid"),
    ),
  notes: z.string().max(notesMaximumLength, translate("fees.validation.notesTooLong")),
});

export type SellClassPackFormValues = z.input<typeof sellClassPackSchema>;

export function toSellClassPackRequest(formValues: SellClassPackFormValues): SellClassPackRequest {
  const notes = formValues.notes.trim();
  return {
    classPackId: formValues.classPackId,
    price: parseAmount(formValues.price) ?? 0,
    purchasedOn: parseBirthDate(formValues.purchasedOn) ?? "",
    method: formValues.method,
    notes: notes.length > 0 ? notes : null,
  };
}

import { z } from "zod";
import { parseAmount } from "@/features/fees/money";
import { PaymentMethod, RecordPaymentRequest } from "@/features/fees/types";
import { isValidBirthDate, parseBirthDate } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";

export const paymentMethods: readonly PaymentMethod[] = ["Cash", "Transfer", "Card", "Other"];

const maximumAmount = 10_000_000;
const notesMaximumLength = 200;

export const amountSchema = z.string().refine((typedAmount) => {
  const amount = parseAmount(typedAmount);
  return amount !== null && amount > 0 && amount <= maximumAmount;
}, translate("fees.validation.amountInvalid"));

export const paymentSchema = z.object({
  amount: amountSchema,
  method: z.enum(paymentMethods as [PaymentMethod, ...PaymentMethod[]]),
  paidOn: z
    .string()
    .refine((paidOn) => isValidBirthDate(paidOn), translate("fees.validation.paidOnInvalid")),
  notes: z.string().max(notesMaximumLength, translate("fees.validation.notesTooLong")),
});

export type PaymentFormValues = z.input<typeof paymentSchema>;

export function toRecordPaymentRequest(
  formValues: PaymentFormValues,
  month: string,
): RecordPaymentRequest {
  const notes = formValues.notes.trim();
  return {
    amount: parseAmount(formValues.amount) ?? 0,
    month,
    paidOn: parseBirthDate(formValues.paidOn) ?? "",
    method: formValues.method,
    notes: notes.length > 0 ? notes : null,
  };
}

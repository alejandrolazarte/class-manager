import { parseAmount } from "@/formatting/money";
import { TranslationKey } from "@/i18n/translate";

const centsPerUnit = 100;

export function paidAmountOf(typedAmount: string): number | null {
  const amount = parseAmount(typedAmount);
  return amount === null ? null : Math.round(amount * centsPerUnit) / centsPerUnit;
}

export function paymentAmountError(
  typedAmount: string,
  balance: number,
  { allowsNothing = false } = {},
): TranslationKey | null {
  if (typedAmount.trim().length === 0) {
    return null;
  }
  const amount = paidAmountOf(typedAmount);
  if (amount === null || (amount === 0 && !allowsNothing)) {
    return "orders.payment.invalidAmount";
  }
  return amount > balance ? "orders.payment.overBalance" : null;
}

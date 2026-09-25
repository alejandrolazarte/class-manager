const moneyLocale = "es-AR";
const argentineThousandsPattern = /^\d{1,3}(\.\d{3})+(,\d{1,2})?$/;
const plainAmountPattern = /^\d+([.,]\d{1,2})?$/;
const thousandsSeparatorPattern = /\./g;
const whitespacePattern = /\s/g;
const decimalComma = ",";
const decimalPoint = ".";

export function formatMoney(amount: number, currencyCode: string): string {
  const isWhole = Number.isInteger(amount);
  return new Intl.NumberFormat(moneyLocale, {
    style: "currency",
    currency: currencyCode,
    minimumFractionDigits: isWhole ? 0 : 2,
    maximumFractionDigits: isWhole ? 0 : 2,
  }).format(amount);
}

export function parseAmount(typedAmount: string): number | null {
  const compactAmount = typedAmount.replace(whitespacePattern, "");
  if (argentineThousandsPattern.test(compactAmount)) {
    return Number(
      compactAmount.replace(thousandsSeparatorPattern, "").replace(decimalComma, decimalPoint),
    );
  }
  if (plainAmountPattern.test(compactAmount)) {
    return Number(compactAmount.replace(decimalComma, decimalPoint));
  }
  return null;
}

export function toAmountText(amount: number | null): string {
  return amount === null || amount <= 0 ? "" : String(amount).replace(decimalPoint, decimalComma);
}

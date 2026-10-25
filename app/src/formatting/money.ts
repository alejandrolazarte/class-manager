const moneyLocale = "es-AR";
const argentineThousandsPattern = /^\d{1,3}(\.\d{3})+(,\d{1,2})?$/;
const plainAmountPattern = /^\d+([.,]\d{1,2})?$/;
const thousandsSeparatorPattern = /\./g;
const whitespacePattern = /\s/g;
const decimalComma = ",";
const decimalPoint = ".";
const usDollarCode = "USD";
const signDisplay = "narrowSymbol";
const usDollarDisplay = "symbol";

function currencyDisplayFor(currencyCode: string): Intl.NumberFormatOptions["currencyDisplay"] {
  return currencyCode === usDollarCode ? usDollarDisplay : signDisplay;
}

export function formatMoney(amount: number, currencyCode: string): string {
  const isWhole = Number.isInteger(amount);
  return new Intl.NumberFormat(moneyLocale, {
    style: "currency",
    currency: currencyCode,
    currencyDisplay: currencyDisplayFor(currencyCode),
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

const currencyPartType = "currency";

export function currencySymbol(currencyCode: string): string {
  const currencyPart = new Intl.NumberFormat(moneyLocale, {
    style: "currency",
    currency: currencyCode,
    currencyDisplay: currencyDisplayFor(currencyCode),
  })
    .formatToParts(0)
    .find((part) => part.type === currencyPartType);
  return currencyPart?.value ?? currencyCode;
}

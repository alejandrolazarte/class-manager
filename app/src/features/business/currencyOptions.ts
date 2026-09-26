const commonCurrencyCodes = ["USD", "EUR"] as const;

export function currencyOptions(
  countryCurrencyCode: string,
  currentCurrencyCode: string,
): string[] {
  return [...new Set([countryCurrencyCode, ...commonCurrencyCodes, currentCurrencyCode])];
}

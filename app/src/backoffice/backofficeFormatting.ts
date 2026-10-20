const formattingLocale = "es-AR";
const dateOnlySeparator = "-";

export function formatMoney(amount: number, currencyCode: string): string {
  const isWhole = Number.isInteger(amount);
  return new Intl.NumberFormat(formattingLocale, {
    style: "currency",
    currency: currencyCode,
    minimumFractionDigits: isWhole ? 0 : 2,
    maximumFractionDigits: isWhole ? 0 : 2,
  }).format(amount);
}

export function formatDateOnly(isoDate: string): string {
  const [year, month, day] = isoDate.split(dateOnlySeparator).map(Number);
  return new Intl.DateTimeFormat(formattingLocale).format(new Date(year, month - 1, day));
}

export function formatDateTimeAsDate(isoDateTime: string): string {
  return new Intl.DateTimeFormat(formattingLocale).format(new Date(isoDateTime));
}

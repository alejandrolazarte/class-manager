const formattingLocale = "es-AR";
const dateOnlySeparator = "-";

export function formatDateOnly(isoDate: string): string {
  const [year, month, day] = isoDate.split(dateOnlySeparator).map(Number);
  return new Intl.DateTimeFormat(formattingLocale).format(new Date(year, month - 1, day));
}

export function formatDateTimeAsDate(isoDateTime: string): string {
  return new Intl.DateTimeFormat(formattingLocale).format(new Date(isoDateTime));
}

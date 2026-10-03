import { translate, TranslationKey } from "@/i18n/translate";

const monthSeparator = "-";

export function monthOf(date: Date = new Date()): string {
  return `${date.getFullYear()}${monthSeparator}${String(date.getMonth() + 1).padStart(2, "0")}`;
}

export function addMonths(month: string, months: number): string {
  const [year, monthNumber] = month.split(monthSeparator).map(Number);
  return monthOf(new Date(year ?? 0, (monthNumber ?? 1) - 1 + months, 1));
}

export function monthName(month: string): string {
  const [, monthNumber] = month.split(monthSeparator).map(Number);
  return translate(`months.${monthNumber ?? 1}` as TranslationKey);
}

export function formatMonth(month: string): string {
  const [year] = month.split(monthSeparator).map(Number);
  const name = monthName(month);
  return `${name.charAt(0).toUpperCase()}${name.slice(1)} ${year}`;
}

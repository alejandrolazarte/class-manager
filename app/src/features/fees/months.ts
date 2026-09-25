import { translate, TranslationKey } from "@/i18n/translate";

const monthSeparator = "-";

export function monthOf(date: Date = new Date()): string {
  return `${date.getFullYear()}${monthSeparator}${String(date.getMonth() + 1).padStart(2, "0")}`;
}

export function addMonths(month: string, months: number): string {
  const [year, monthNumber] = month.split(monthSeparator).map(Number);
  return monthOf(new Date(year ?? 0, (monthNumber ?? 1) - 1 + months, 1));
}

export function formatMonth(month: string): string {
  const [year, monthNumber] = month.split(monthSeparator).map(Number);
  const monthName = translate(`months.${monthNumber ?? 1}` as TranslationKey);
  return `${monthName.charAt(0).toUpperCase()}${monthName.slice(1)} ${year}`;
}

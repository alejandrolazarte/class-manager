import { weekdayLongLabel, weekdayOf } from "@/features/classGroups/weekdays";
import { translate, TranslationKey } from "@/i18n/translate";

const isoDateSeparator = "-";

export function parseIsoDate(isoDate: string): Date {
  const [year, month, day] = isoDate.split(isoDateSeparator).map(Number);
  return new Date(year ?? 0, (month ?? 1) - 1, day ?? 1);
}

export function toIsoDate(date: Date): string {
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${date.getFullYear()}${isoDateSeparator}${month}${isoDateSeparator}${day}`;
}

export function todayIsoDate(today: Date = new Date()): string {
  return toIsoDate(today);
}

export function addDays(isoDate: string, days: number): string {
  const date = parseIsoDate(isoDate);
  date.setDate(date.getDate() + days);
  return toIsoDate(date);
}

export function formatLongDate(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  return translate("dates.long", {
    weekday: weekdayLongLabel(weekdayOf(date)),
    day: date.getDate(),
    month: translate(`months.${date.getMonth() + 1}` as TranslationKey),
  });
}

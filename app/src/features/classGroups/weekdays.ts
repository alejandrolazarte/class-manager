import { Weekday } from "@/features/classGroups/types";
import { translate, TranslationKey } from "@/i18n/translate";

export const weekOrder: readonly Weekday[] = [
  "Monday",
  "Tuesday",
  "Wednesday",
  "Thursday",
  "Friday",
  "Saturday",
  "Sunday",
];

const weekdayByJavaScriptDay: readonly Weekday[] = [
  "Sunday",
  "Monday",
  "Tuesday",
  "Wednesday",
  "Thursday",
  "Friday",
  "Saturday",
];

export function weekdayShortLabel(weekday: Weekday): string {
  return translate(`weekdays.short.${weekday}` as TranslationKey);
}

export function weekdayLetter(weekday: Weekday): string {
  return translate(`weekdays.letter.${weekday}` as TranslationKey);
}

export function weekdayLongLabel(weekday: Weekday): string {
  return translate(`weekdays.long.${weekday}` as TranslationKey);
}

export function weekdayOf(date: Date): Weekday {
  return weekdayByJavaScriptDay[date.getDay()] ?? "Monday";
}

export function sortWeekdays(weekdays: readonly Weekday[]): Weekday[] {
  return weekOrder.filter((weekday) => weekdays.includes(weekday));
}

export function isWeekday(value: string | undefined): value is Weekday {
  return (weekOrder as readonly string[]).includes(value ?? "");
}

import { weekdayLongLabel, weekdayOf } from "@/dates/weekdays";
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

export function formatClockTime(date: Date): string {
  return `${String(date.getHours()).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}`;
}

export function todayIsoDate(today: Date = new Date()): string {
  return toIsoDate(today);
}

export function addDays(isoDate: string, days: number): string {
  const date = parseIsoDate(isoDate);
  date.setDate(date.getDate() + days);
  return toIsoDate(date);
}

export function addMonthsToDate(isoDate: string, months: number): string {
  const date = parseIsoDate(isoDate);
  const targetMonthIndex = date.getMonth() + months;
  const lastDayOfTargetMonth = new Date(date.getFullYear(), targetMonthIndex + 1, 0).getDate();
  return toIsoDate(
    new Date(date.getFullYear(), targetMonthIndex, Math.min(date.getDate(), lastDayOfTargetMonth)),
  );
}

export function formatLongDate(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  return translate("dates.long", {
    weekday: weekdayLongLabel(weekdayOf(date)),
    day: date.getDate(),
    month: translate(`months.${date.getMonth() + 1}` as TranslationKey),
  });
}

const daysPerWeek = 7;
const javaScriptMondayOffset = 6;

export function weekOf(isoDate: string): string[] {
  const date = parseIsoDate(isoDate);
  const daysSinceMonday = (date.getDay() + javaScriptMondayOffset) % daysPerWeek;
  const monday = addDays(isoDate, -daysSinceMonday);
  return Array.from({ length: daysPerWeek }, (_, dayIndex) => addDays(monday, dayIndex));
}

export function dayOfMonth(isoDate: string): number {
  return parseIsoDate(isoDate).getDate();
}

const monthGridWeeks = 6;
const firstDayOfMonth = "01";

export function monthOfDate(isoDate: string): string {
  return isoDate.slice(0, 7);
}

export function monthGridOf(month: string): string[] {
  const [monday] = weekOf(`${month}${isoDateSeparator}${firstDayOfMonth}`);
  return Array.from({ length: monthGridWeeks * daysPerWeek }, (_, dayIndex) =>
    addDays(monday ?? `${month}${isoDateSeparator}${firstDayOfMonth}`, dayIndex),
  );
}

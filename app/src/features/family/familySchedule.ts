import { weekdayShortLabel, weekdayOf } from "@/features/classGroups/weekdays";
import { FamilyNextClass, FamilyStudent } from "@/features/family/types";
import { addDays, dayOfMonth, parseIsoDate, todayIsoDate } from "@/features/sessions/dates";
import { translate, TranslationKey } from "@/i18n/translate";

const nameSeparatorPattern = /\s+/;
const detailSeparator = " · ";

export function firstNameOf(fullName: string): string {
  return fullName.trim().split(nameSeparatorPattern)[0] ?? fullName;
}

const minutesPerHour = 60;
const timeSeparator = ":";

function minutesOfDay(time: string): number {
  const [hours, minutes] = time.split(timeSeparator).map(Number);
  return (hours ?? 0) * minutesPerHour + (minutes ?? 0);
}

function minutesOfDayNow(now: Date): number {
  return now.getHours() * minutesPerHour + now.getMinutes();
}

function hasEnded(nextClass: FamilyNextClass, now: Date): boolean {
  return (
    nextClass.date < todayIsoDate(now) ||
    (nextClass.date === todayIsoDate(now) &&
      minutesOfDay(nextClass.endTime) <= minutesOfDayNow(now))
  );
}

export function nextClassOf(
  student: FamilyStudent,
  now: Date = new Date(),
): FamilyNextClass | null {
  return (
    student.nextClasses.find((nextClass) => !nextClass.isCancelled && !hasEnded(nextClass, now)) ??
    null
  );
}

export function classesOn(student: FamilyStudent, isoDate: string): FamilyNextClass[] {
  return student.nextClasses.filter((nextClass) => nextClass.date === isoDate);
}

export function shortDayLabel(isoDate: string): string {
  return `${weekdayShortLabel(weekdayOf(parseIsoDate(isoDate)))} ${dayOfMonth(isoDate)}`;
}

export function relativeDayLabel(isoDate: string, today: string = todayIsoDate()): string {
  if (isoDate === today) {
    return translate("family.day.today");
  }
  if (isoDate === addDays(today, 1)) {
    return translate("family.day.tomorrow");
  }
  return shortDayLabel(isoDate);
}

const millisecondsPerDay = 86_400_000;

export function countdownLabel(nextClass: FamilyNextClass, now: Date = new Date()): string | null {
  const today = todayIsoDate(now);
  const days = Math.round(
    (parseIsoDate(nextClass.date).getTime() - parseIsoDate(today).getTime()) / millisecondsPerDay,
  );
  if (days <= 0) {
    const minutesLeft = minutesOfDay(nextClass.startTime) - minutesOfDayNow(now);
    if (minutesLeft <= 0) {
      return translate("family.nextClass.now");
    }
    return minutesLeft < minutesPerHour
      ? translate("family.nextClass.inMinutes", { minutes: minutesLeft })
      : translate("family.nextClass.inHours", {
          hours: Math.floor(minutesLeft / minutesPerHour),
          minutes: minutesLeft % minutesPerHour,
        });
  }
  if (days === 1) {
    return null;
  }
  return translate("family.nextClass.inDays", { count: days });
}

export function classTitle(nextClass: FamilyNextClass): string {
  return nextClass.isPrivateLesson ? translate("family.student.privateLesson") : nextClass.name;
}

export function classDetails(nextClass: FamilyNextClass): string {
  const withInstructor = nextClass.instructorFullName
    ? `${classTitle(nextClass)} ${translate("family.student.with", { instructor: nextClass.instructorFullName })}`
    : classTitle(nextClass);
  return [withInstructor, nextClass.location].filter(Boolean).join(detailSeparator);
}

export function monthTitle(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  const month = translate(`months.${date.getMonth() + 1}` as TranslationKey);
  return translate("family.schedule.monthTitle", {
    month: month.charAt(0).toUpperCase() + month.slice(1),
    year: date.getFullYear(),
  });
}

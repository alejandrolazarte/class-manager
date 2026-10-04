import { weekdayShortLabel, weekdayOf } from "@/features/classGroups/weekdays";
import { StudentAppNextClass, AccountStudent } from "@/features/studentApp/types";
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

function hasEnded(nextClass: StudentAppNextClass, now: Date): boolean {
  return (
    nextClass.date < todayIsoDate(now) ||
    (nextClass.date === todayIsoDate(now) &&
      minutesOfDay(nextClass.endTime) <= minutesOfDayNow(now))
  );
}

export function hasStarted(nextClass: StudentAppNextClass, now: Date = new Date()): boolean {
  return (
    nextClass.date < todayIsoDate(now) ||
    (nextClass.date === todayIsoDate(now) &&
      minutesOfDay(nextClass.startTime) <= minutesOfDayNow(now))
  );
}

export function canNotifyAbsence(nextClass: StudentAppNextClass, now: Date = new Date()): boolean {
  return (
    nextClass.classGroupId !== null &&
    !nextClass.isMakeup &&
    !nextClass.isPackBooking &&
    !nextClass.isCancelled &&
    !hasStarted(nextClass, now)
  );
}

export function canCancelMakeup(nextClass: StudentAppNextClass, now: Date = new Date()): boolean {
  return nextClass.isMakeup && !nextClass.isCancelled && !hasStarted(nextClass, now);
}

export function canCancelPackClass(
  nextClass: StudentAppNextClass,
  now: Date = new Date(),
): boolean {
  return nextClass.isPackBooking && !nextClass.isCancelled && !hasStarted(nextClass, now);
}

export function nextClassOf(
  student: AccountStudent,
  now: Date = new Date(),
): StudentAppNextClass | null {
  return (
    student.nextClasses.find((nextClass) => !nextClass.isCancelled && !hasEnded(nextClass, now)) ??
    null
  );
}

export function classesOn(student: AccountStudent, isoDate: string): StudentAppNextClass[] {
  return student.nextClasses.filter((nextClass) => nextClass.date === isoDate);
}

export function shortDayLabel(isoDate: string): string {
  return `${weekdayShortLabel(weekdayOf(parseIsoDate(isoDate)))} ${dayOfMonth(isoDate)}`;
}

export function dayAndMonthLabel(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  return translate("student.makeup.dayAndMonth", {
    day: date.getDate(),
    month: translate(`months.${date.getMonth() + 1}` as TranslationKey),
  });
}

export function relativeDayLabel(isoDate: string, today: string = todayIsoDate()): string {
  if (isoDate === today) {
    return translate("student.day.today");
  }
  if (isoDate === addDays(today, 1)) {
    return translate("student.day.tomorrow");
  }
  return shortDayLabel(isoDate);
}

const millisecondsPerDay = 86_400_000;

export function countdownLabel(
  nextClass: StudentAppNextClass,
  now: Date = new Date(),
): string | null {
  const today = todayIsoDate(now);
  const days = Math.round(
    (parseIsoDate(nextClass.date).getTime() - parseIsoDate(today).getTime()) / millisecondsPerDay,
  );
  if (days <= 0) {
    const minutesLeft = minutesOfDay(nextClass.startTime) - minutesOfDayNow(now);
    if (minutesLeft <= 0) {
      return translate("student.nextClass.now");
    }
    return minutesLeft < minutesPerHour
      ? translate("student.nextClass.inMinutes", { minutes: minutesLeft })
      : translate("student.nextClass.inHours", {
          hours: Math.floor(minutesLeft / minutesPerHour),
          minutes: minutesLeft % minutesPerHour,
        });
  }
  if (days === 1) {
    return null;
  }
  return translate("student.nextClass.inDays", { count: days });
}

export function classTitle(nextClass: StudentAppNextClass): string {
  return nextClass.isPrivateLesson ? translate("student.student.privateLesson") : nextClass.name;
}

export function classDetails(nextClass: StudentAppNextClass): string {
  const withInstructor = nextClass.instructorFullName
    ? `${classTitle(nextClass)} ${translate("student.student.with", { instructor: nextClass.instructorFullName })}`
    : classTitle(nextClass);
  return [withInstructor, nextClass.location].filter(Boolean).join(detailSeparator);
}

export function monthTitle(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  const month = translate(`months.${date.getMonth() + 1}` as TranslationKey);
  return translate("student.schedule.monthTitle", {
    month: month.charAt(0).toUpperCase() + month.slice(1),
    year: date.getFullYear(),
  });
}

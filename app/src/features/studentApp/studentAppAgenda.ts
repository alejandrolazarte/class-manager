import { addDays, monthOfDate, weekOf } from "@/dates/dates";
import { classesOn } from "@/features/studentApp/studentAppSchedule";
import { AccountStudent, PastClassStatus, StudentAppPastClass } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AgendaView } from "@/ui/AgendaCalendar";
import { DayMarker } from "@/ui/DayMarker";
import { StatusTone } from "@/ui/StatusPill";

const daysPerWeek = 7;
const lookBackWeeks = 52;
const lookAheadWeeks = 2;

export function firstAgendaWeekStart(today: string): string {
  return addDays(weekOf(today)[0] ?? today, -lookBackWeeks * daysPerWeek);
}

export function lastAgendaWeekStart(today: string): string {
  return addDays(weekOf(today)[0] ?? today, lookAheadWeeks * daysPerWeek);
}

export function lastAgendaDay(today: string): string {
  return addDays(lastAgendaWeekStart(today), daysPerWeek - 1);
}

export function isFirstAgendaPage(view: AgendaView, selectedDate: string, today: string): boolean {
  const firstWeekStart = firstAgendaWeekStart(today);
  return view === "month"
    ? monthOfDate(selectedDate) <= monthOfDate(firstWeekStart)
    : (weekOf(selectedDate)[0] ?? selectedDate) <= firstWeekStart;
}

export function isLastAgendaPage(view: AgendaView, selectedDate: string, today: string): boolean {
  return view === "month"
    ? monthOfDate(selectedDate) >= monthOfDate(lastAgendaDay(today))
    : (weekOf(selectedDate)[0] ?? selectedDate) >= lastAgendaWeekStart(today);
}

export function historyMonthsOf(view: AgendaView, selectedDate: string, today: string): string[] {
  const lastPastMonth = monthOfDate(addDays(today, -1));
  const visibleMonths =
    view === "month"
      ? [monthOfDate(selectedDate)]
      : weekOf(selectedDate)
          .filter((isoDate) => isoDate < today)
          .map((isoDate) => monthOfDate(isoDate));
  return [...new Set(visibleMonths)].filter((month) => month <= lastPastMonth);
}

export function pastClassesOn(
  pastClasses: readonly StudentAppPastClass[],
  isoDate: string,
): StudentAppPastClass[] {
  return pastClasses.filter((pastClass) => pastClass.date === isoDate);
}

export function agendaMarkerOf(
  isoDate: string,
  today: string,
  student: AccountStudent,
  pastClasses: readonly StudentAppPastClass[],
): DayMarker {
  if (isoDate >= today) {
    return classesOn(student, isoDate).some((nextClass) => !nextClass.isCancelled)
      ? "classes"
      : "none";
  }
  const dayClasses = pastClassesOn(pastClasses, isoDate);
  if (dayClasses.some((pastClass) => pastClass.status === "Absent")) {
    return "missed";
  }
  if (dayClasses.some((pastClass) => pastClass.status === "Attended")) {
    return "attended";
  }
  return dayClasses.some((pastClass) => pastClass.status !== "Cancelled") ? "classes" : "none";
}

export function isCancelledDay(
  isoDate: string,
  today: string,
  student: AccountStudent,
  pastClasses: readonly StudentAppPastClass[],
): boolean {
  const statuses =
    isoDate >= today
      ? classesOn(student, isoDate).map((nextClass) => nextClass.isCancelled)
      : pastClassesOn(pastClasses, isoDate).map((pastClass) => pastClass.status === "Cancelled");
  return statuses.length > 0 && statuses.every(Boolean);
}

export function heldAndAttendedCount(
  pastClasses: readonly StudentAppPastClass[],
  month: string,
): { held: number; attended: number } {
  const monthClasses = pastClasses.filter(
    (pastClass) => monthOfDate(pastClass.date) === month && pastClass.status !== "Cancelled",
  );
  return {
    held: monthClasses.length,
    attended: monthClasses.filter((pastClass) => pastClass.status === "Attended").length,
  };
}

const statusLabelKeys: Record<PastClassStatus, TranslationKey> = {
  Attended: "student.history.attended",
  Absent: "student.history.absent",
  AbsenceNotified: "student.history.absenceNotified",
  Cancelled: "student.student.cancelled",
  Unmarked: "student.history.unmarked",
};

const statusTones: Record<PastClassStatus, StatusTone> = {
  Attended: "success",
  Absent: "danger",
  AbsenceNotified: "neutral",
  Cancelled: "danger",
  Unmarked: "neutral",
};

export function pastClassStatusOf(pastClass: StudentAppPastClass): [string, StatusTone] {
  const labelKey =
    pastClass.status === "Attended" && pastClass.isMakeup
      ? "student.history.madeUp"
      : statusLabelKeys[pastClass.status];
  return [translate(labelKey), statusTones[pastClass.status]];
}

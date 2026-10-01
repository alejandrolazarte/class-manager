import { DaySession } from "@/features/sessions/types";
import { translate, translateCount } from "@/i18n/translate";

export type SessionTiming = "cancelled" | "live" | "ended" | "upcoming";

export type TodayHighlight =
  { kind: "live"; session: DaySession } | { kind: "next"; session: DaySession } | { kind: "none" };

const timeDigits = 2;
const minutesPerHour = 60;
const millisecondsPerDay = 86_400_000;
const timeSeparator = ":";

export function currentTimeLabel(now: Date = new Date()): string {
  return `${String(now.getHours()).padStart(timeDigits, "0")}:${String(now.getMinutes()).padStart(timeDigits, "0")}`;
}

function minutesOf(time: string): number {
  const [hours, minutes] = time.split(timeSeparator).map(Number);
  return (hours ?? 0) * minutesPerHour + (minutes ?? 0);
}

function utcDayOf(isoDate: string): number {
  const [year, month, day] = isoDate.split("-").map(Number);
  return Date.UTC(year ?? 1970, (month ?? 1) - 1, day ?? 1);
}

export function daysBetween(fromIsoDate: string, toIsoDate: string): number {
  return Math.round((utcDayOf(toIsoDate) - utcDayOf(fromIsoDate)) / millisecondsPerDay);
}

export function sessionTimingOf(
  session: DaySession,
  today: string,
  timeNow: string,
): SessionTiming {
  if (session.isCancelled) {
    return "cancelled";
  }
  if (session.date !== today) {
    return session.date < today ? "ended" : "upcoming";
  }
  if (minutesOf(session.endTime) <= minutesOf(timeNow)) {
    return "ended";
  }
  return minutesOf(session.startTime) <= minutesOf(timeNow) ? "live" : "upcoming";
}

export function startsInLabel(startTime: string, timeNow: string): string {
  const minutes = Math.max(minutesOf(startTime) - minutesOf(timeNow), 0);
  const hours = Math.floor(minutes / minutesPerHour);
  const remainingMinutes = minutes % minutesPerHour;
  if (hours === 0) {
    return translate("home.agenda.inMinutes", { minutes });
  }
  return remainingMinutes === 0
    ? translate("home.agenda.inHours", { hours })
    : translate("home.agenda.inHoursAndMinutes", { hours, minutes: remainingMinutes });
}

export function relativeDayLabel(isoDate: string, today: string): string {
  const difference = daysBetween(today, isoDate);
  if (difference === 0) {
    return translate("sessions.day.title");
  }
  if (difference === 1) {
    return translate("sessions.day.tomorrow");
  }
  if (difference === -1) {
    return translate("sessions.day.yesterday");
  }
  return difference > 0
    ? translateCount("home.agenda.inDays", difference)
    : translateCount("home.agenda.daysAgo", -difference);
}

export function todayHighlightOf(
  sessions: readonly DaySession[],
  today: string,
  timeNow: string,
): TodayHighlight {
  const byStart = [...sessions].sort((first, second) =>
    first.startTime.localeCompare(second.startTime),
  );
  const live = byStart.find((session) => sessionTimingOf(session, today, timeNow) === "live");
  if (live !== undefined) {
    return { kind: "live", session: live };
  }
  const next = byStart.find((session) => sessionTimingOf(session, today, timeNow) === "upcoming");
  return next === undefined ? { kind: "none" } : { kind: "next", session: next };
}

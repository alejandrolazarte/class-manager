import { DaySession } from "@/features/sessions/types";

const timeDigits = 2;

export function currentTimeLabel(now: Date = new Date()): string {
  return `${String(now.getHours()).padStart(timeDigits, "0")}:${String(now.getMinutes()).padStart(timeDigits, "0")}`;
}

export function nextSessionOf(sessions: readonly DaySession[], timeNow: string): DaySession | null {
  return (
    [...sessions]
      .filter((session) => !session.isCancelled && session.endTime > timeNow)
      .sort((first, second) => first.startTime.localeCompare(second.startTime))[0] ?? null
  );
}

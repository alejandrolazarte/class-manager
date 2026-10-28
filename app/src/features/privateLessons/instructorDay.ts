import {
  minutesPerDay,
  startTimePattern,
  toMinutesOfDay,
} from "@/features/classGroups/startTimeFormatting";
import { InstructorAgendaSlot } from "@/features/privateLessons/types";

const minutesPerHour = 60;
const visibleMarginMinutes = 60;

interface MinutesRange {
  startMinutes: number;
  endMinutes: number;
}

export interface BusyPeriod extends MinutesRange {
  slot: InstructorAgendaSlot;
}

export interface FreeGap extends MinutesRange {
  suggestedStartMinutes: number;
  isOpenStart: boolean;
  isOpenEnd: boolean;
}

export interface PlannedLesson extends MinutesRange {
  conflict: BusyPeriod | null;
}

export interface InstructorDay {
  visibleStartMinutes: number;
  visibleEndMinutes: number;
  busyPeriods: BusyPeriod[];
  freeGaps: FreeGap[];
  lesson: PlannedLesson | null;
}

function overlaps(first: MinutesRange, second: MinutesRange): boolean {
  return first.startMinutes < second.endMinutes && second.startMinutes < first.endMinutes;
}

export function isLessonInsideGap(lesson: PlannedLesson | null, gap: FreeGap): boolean {
  return (
    lesson !== null &&
    lesson.conflict === null &&
    lesson.startMinutes >= gap.startMinutes &&
    lesson.endMinutes <= gap.endMinutes
  );
}

function toBusyPeriod(slot: InstructorAgendaSlot): BusyPeriod {
  const startMinutes = toMinutesOfDay(slot.startTime);
  return { startMinutes, endMinutes: startMinutes + slot.durationMinutes, slot };
}

function planLesson(
  busyPeriods: BusyPeriod[],
  durationMinutes: number | null,
  startTime: string,
): PlannedLesson | null {
  if (durationMinutes === null || !startTimePattern.test(startTime)) {
    return null;
  }
  const startMinutes = toMinutesOfDay(startTime);
  const lessonRange = { startMinutes, endMinutes: startMinutes + durationMinutes };
  const conflict = busyPeriods.find((busyPeriod) => overlaps(busyPeriod, lessonRange)) ?? null;
  return { ...lessonRange, conflict };
}

function findFreeGaps(busyPeriods: BusyPeriod[], durationMinutes: number | null): FreeGap[] {
  if (durationMinutes === null) {
    return [];
  }
  const freeRanges: MinutesRange[] = [];
  let freeSince = 0;
  for (const busyPeriod of busyPeriods) {
    if (busyPeriod.startMinutes > freeSince) {
      freeRanges.push({ startMinutes: freeSince, endMinutes: busyPeriod.startMinutes });
    }
    freeSince = Math.max(freeSince, busyPeriod.endMinutes);
  }
  if (freeSince < minutesPerDay) {
    freeRanges.push({ startMinutes: freeSince, endMinutes: minutesPerDay });
  }
  return freeRanges
    .filter((freeRange) => freeRange.endMinutes - freeRange.startMinutes >= durationMinutes)
    .map((freeRange) => {
      const isOpenStart = freeRange.startMinutes === 0;
      return {
        ...freeRange,
        suggestedStartMinutes: isOpenStart
          ? freeRange.endMinutes - durationMinutes
          : freeRange.startMinutes,
        isOpenStart,
        isOpenEnd: freeRange.endMinutes === minutesPerDay,
      };
    });
}

function floorToHour(minutes: number): number {
  return Math.max(0, Math.floor(minutes / minutesPerHour) * minutesPerHour);
}

function ceilToHour(minutes: number): number {
  return Math.min(minutesPerDay, Math.ceil(minutes / minutesPerHour) * minutesPerHour);
}

export function planInstructorDay(
  slots: InstructorAgendaSlot[],
  durationMinutes: number | null,
  startTime: string,
): InstructorDay {
  const busyPeriods = slots
    .map(toBusyPeriod)
    .sort((first, second) => first.startMinutes - second.startMinutes);
  const lesson = planLesson(busyPeriods, durationMinutes, startTime);
  const firstBusyPeriod = busyPeriods[0];
  const latestBusyMinutes = Math.max(...busyPeriods.map((busyPeriod) => busyPeriod.endMinutes));
  const visibleStartMinutes =
    firstBusyPeriod === undefined
      ? minutesPerDay
      : floorToHour(firstBusyPeriod.startMinutes - visibleMarginMinutes);
  const visibleEndMinutes =
    firstBusyPeriod === undefined ? 0 : ceilToHour(latestBusyMinutes + visibleMarginMinutes);
  return {
    visibleStartMinutes:
      lesson === null
        ? visibleStartMinutes
        : Math.min(visibleStartMinutes, floorToHour(lesson.startMinutes)),
    visibleEndMinutes:
      lesson === null
        ? visibleEndMinutes
        : Math.max(visibleEndMinutes, ceilToHour(lesson.endMinutes)),
    busyPeriods,
    freeGaps: findFreeGaps(busyPeriods, durationMinutes),
    lesson,
  };
}

import { sortWeekdays, Weekday } from "@/dates/weekdays";
import { InstructorWeekSlot } from "@/features/classGroups/types";
import { FreeGap, InstructorDay, planInstructorDay } from "@/features/privateLessons/instructorDay";

export interface InstructorWeekday extends InstructorDay {
  weekday: Weekday;
}

export interface InstructorWeek {
  visibleStartMinutes: number;
  visibleEndMinutes: number;
  days: InstructorWeekday[];
  freeGapsOnEveryDay: FreeGap[];
  hasClasses: boolean;
  isClassPlanned: boolean;
  clashingDayCount: number;
}

export function planInstructorWeek(
  slots: InstructorWeekSlot[],
  weekdays: Weekday[],
  durationMinutes: number | null,
  startTime: string,
): InstructorWeek {
  const days = sortWeekdays(weekdays).map((weekday): InstructorWeekday => ({
    weekday,
    ...planInstructorDay(
      slots.filter((slot) => slot.weekday === weekday),
      durationMinutes,
      startTime,
    ),
  }));
  const slotsOfTheDays = slots.filter((slot) => weekdays.includes(slot.weekday));
  const sharedDay = planInstructorDay(slotsOfTheDays, durationMinutes, startTime);
  return {
    visibleStartMinutes: sharedDay.visibleStartMinutes,
    visibleEndMinutes: sharedDay.visibleEndMinutes,
    days,
    freeGapsOnEveryDay: sharedDay.freeGaps,
    hasClasses: slotsOfTheDays.length > 0,
    isClassPlanned: sharedDay.lesson !== null,
    clashingDayCount: days.filter((day) => day.lesson?.conflict).length,
  };
}

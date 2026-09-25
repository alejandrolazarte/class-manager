import { ClassGroup, Weekday } from "@/features/classGroups/types";

export function classesOfDay(classGroups: readonly ClassGroup[], weekday: Weekday): ClassGroup[] {
  return classGroups
    .filter((classGroup) => classGroup.isActive && classGroup.weekdays.includes(weekday))
    .sort(
      (first, second) =>
        first.startTime.localeCompare(second.startTime) || first.name.localeCompare(second.name),
    );
}

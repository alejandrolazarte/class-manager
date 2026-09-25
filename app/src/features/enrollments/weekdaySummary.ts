import { Weekday } from "@/features/classGroups/types";
import { sortWeekdays, weekdayShortLabel } from "@/features/classGroups/weekdays";
import { translate } from "@/i18n/translate";

const listSeparator = ", ";

export function summarizeWeekdays(weekdays: readonly Weekday[]): string {
  const labels = sortWeekdays(weekdays).map(weekdayShortLabel);
  const lastLabel = labels.pop();
  if (lastLabel === undefined) {
    return "";
  }
  return labels.length === 0
    ? lastLabel
    : `${labels.join(listSeparator)}${translate("enrollments.lastItemJoiner")}${lastLabel}`;
}

export function summarizeSchedule(weekdays: readonly Weekday[], startTime: string): string {
  return `${summarizeWeekdays(weekdays)} ${startTime}`;
}

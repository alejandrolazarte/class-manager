import { weekdayShortLabel, weekdayOf } from "@/features/classGroups/weekdays";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { dayOfMonth, parseIsoDate } from "@/features/sessions/dates";
import { DayStrip } from "@/ui/DayStrip";

interface WeekStripProps {
  days: readonly string[];
  today: string;
  selectedDate: string;
  hasClassesOn: (isoDate: string) => boolean;
  onSelect: (isoDate: string) => void;
}

export function WeekStrip({ days, today, selectedDate, hasClassesOn, onSelect }: WeekStripProps) {
  return (
    <DayStrip
      days={days.map((isoDate) => ({
        key: isoDate,
        label: weekdayShortLabel(weekdayOf(parseIsoDate(isoDate))),
        accessibilityLabel: shortDayLabel(isoDate),
        dayNumber: dayOfMonth(isoDate),
        hasClasses: hasClassesOn(isoDate),
        isToday: isoDate === today,
      }))}
      selectedKeys={[selectedDate]}
      onSelect={onSelect}
    />
  );
}

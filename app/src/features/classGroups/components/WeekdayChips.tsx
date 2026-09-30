import { Weekday } from "@/features/classGroups/types";
import { weekdayLetter, weekdayLongLabel, weekOrder } from "@/features/classGroups/weekdays";
import { DayStrip } from "@/ui/DayStrip";

interface WeekdayChipsProps {
  selectedWeekdays: readonly Weekday[];
  onToggleWeekday: (weekday: Weekday) => void;
}

export function WeekdayChips({ selectedWeekdays, onToggleWeekday }: WeekdayChipsProps) {
  return (
    <DayStrip
      days={weekOrder.map((weekday) => ({
        key: weekday,
        label: weekdayLetter(weekday),
        accessibilityLabel: weekdayLongLabel(weekday),
      }))}
      selectedKeys={selectedWeekdays}
      onSelect={(weekday) => onToggleWeekday(weekday as Weekday)}
      showsClassDots={false}
    />
  );
}

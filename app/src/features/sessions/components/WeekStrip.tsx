import { weekdayLongLabel, weekdayOf, weekdayShortLabel } from "@/features/classGroups/weekdays";
import { dayOfMonth, parseIsoDate, weekOf } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { DayStrip } from "@/ui/DayStrip";

interface WeekStripProps {
  selectedDate: string;
  today: string;
  hasClassesOn: (isoDate: string) => boolean;
  onSelectDate: (isoDate: string) => void;
}

export function WeekStrip({ selectedDate, today, hasClassesOn, onSelectDate }: WeekStripProps) {
  return (
    <DayStrip
      days={weekOf(selectedDate).map((isoDate) => {
        const weekday = weekdayOf(parseIsoDate(isoDate));
        return {
          key: isoDate,
          label: weekdayShortLabel(weekday),
          accessibilityLabel: translate("sessions.day.pickDay", {
            weekday: weekdayLongLabel(weekday),
            day: dayOfMonth(isoDate),
          }),
          dayNumber: dayOfMonth(isoDate),
          hasClasses: hasClassesOn(isoDate),
          isToday: isoDate === today,
        };
      })}
      selectedKeys={[selectedDate]}
      onSelect={onSelectDate}
    />
  );
}

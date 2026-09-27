import { Pressable, View } from "react-native";
import {
  weekdayLongLabel,
  weekdayOf,
  weekdayShortLabel,
  weekOrder,
} from "@/features/classGroups/weekdays";
import { dayOfMonth, monthGridOf, monthOfDate, parseIsoDate } from "@/features/sessions/dates";
import { CalendarDay } from "@/features/sessions/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

interface MonthCalendarGridProps {
  month: string;
  days: readonly CalendarDay[];
  selectedDate: string;
  today: string;
  onSelectDate: (isoDate: string) => void;
}

interface DayCellProps {
  isoDate: string;
  calendarDay: CalendarDay | undefined;
  isInMonth: boolean;
  isSelected: boolean;
  isToday: boolean;
  onPress: () => void;
}

const daysPerWeek = 7;
const labelSeparator = " · ";

function dayAccessibilityLabel(isoDate: string, calendarDay: CalendarDay | undefined): string {
  const parts = [
    translate("sessions.calendar.dayLabel", {
      weekday: weekdayLongLabel(weekdayOf(parseIsoDate(isoDate))),
      day: dayOfMonth(isoDate),
    }),
  ];
  if (calendarDay) {
    parts.push(translateCount("sessions.day.classCount", calendarDay.classCount));
    if (calendarDay.pendingAttendanceCount > 0) {
      parts.push(translate("sessions.calendar.pending"));
    }
    if (calendarDay.cancelledCount > 0) {
      parts.push(translate("sessions.calendar.cancelled"));
    }
  }
  return parts.join(labelSeparator);
}

function DayCell({ isoDate, calendarDay, isInMonth, isSelected, isToday, onPress }: DayCellProps) {
  const hasCancellation = (calendarDay?.cancelledCount ?? 0) > 0;
  const hasPendingAttendance = (calendarDay?.pendingAttendanceCount ?? 0) > 0;
  const hasClasses = (calendarDay?.classCount ?? 0) > 0;
  const backgroundClassName = isSelected
    ? "bg-primary"
    : hasCancellation
      ? "bg-danger-soft"
      : "bg-transparent";
  const markerClassName = hasPendingAttendance
    ? "bg-warning"
    : hasClasses
      ? isSelected
        ? "bg-primary-foreground"
        : "bg-primary"
      : "bg-transparent";
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={dayAccessibilityLabel(isoDate, calendarDay)}
      accessibilityState={{ selected: isSelected }}
      onPress={onPress}
      className={`h-12 flex-1 items-center justify-center gap-1 rounded-xl border-[1.5px] ${isToday && !isSelected ? "border-primary" : "border-transparent"} ${backgroundClassName} ${isInMonth ? "" : "opacity-40"}`}
    >
      <AppText
        variant="bodyStrong"
        tone={isSelected ? "onPrimary" : hasCancellation ? "dangerSoft" : "default"}
      >
        {dayOfMonth(isoDate)}
      </AppText>
      <View className={`h-1.5 w-1.5 rounded-full ${markerClassName}`} />
    </Pressable>
  );
}

function LegendItem({ markerClassName, label }: { markerClassName: string; label: string }) {
  return (
    <View className="flex-row items-center gap-1.5">
      <View className={`h-2.5 w-2.5 rounded-full ${markerClassName}`} />
      <AppText variant="footnote" tone="muted" className="font-label">
        {label}
      </AppText>
    </View>
  );
}

export function MonthCalendarGrid({
  month,
  days,
  selectedDate,
  today,
  onSelectDate,
}: MonthCalendarGridProps) {
  const calendarDays = new Map(days.map((calendarDay) => [calendarDay.date, calendarDay]));
  const gridDates = monthGridOf(month);
  const weeks = Array.from({ length: gridDates.length / daysPerWeek }, (_, weekIndex) =>
    gridDates.slice(weekIndex * daysPerWeek, (weekIndex + 1) * daysPerWeek),
  ).filter((week) => week.some((isoDate) => monthOfDate(isoDate) === month));
  return (
    <Card className="gap-1.5 p-3">
      <View className="flex-row">
        {weekOrder.map((weekday) => (
          <AppText
            key={weekday}
            variant="footnote"
            tone="subtle"
            className="flex-1 text-center font-label"
          >
            {weekdayShortLabel(weekday)}
          </AppText>
        ))}
      </View>
      {weeks.map((week) => (
        <View key={week[0]} className="flex-row gap-1">
          {week.map((isoDate) => (
            <DayCell
              key={isoDate}
              isoDate={isoDate}
              calendarDay={calendarDays.get(isoDate)}
              isInMonth={monthOfDate(isoDate) === month}
              isSelected={isoDate === selectedDate}
              isToday={isoDate === today}
              onPress={() => onSelectDate(isoDate)}
            />
          ))}
        </View>
      ))}
      <View className="flex-row flex-wrap gap-x-4 gap-y-1.5 px-1 pt-2">
        <LegendItem
          markerClassName="bg-primary"
          label={translate("sessions.calendar.legendClasses")}
        />
        <LegendItem
          markerClassName="bg-warning"
          label={translate("sessions.calendar.legendPending")}
        />
        <LegendItem
          markerClassName="bg-danger-soft border border-danger"
          label={translate("sessions.calendar.legendCancelled")}
        />
      </View>
    </Card>
  );
}

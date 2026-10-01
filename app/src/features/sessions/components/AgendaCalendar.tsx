import { Pressable, View } from "react-native";
import {
  weekdayLetter,
  weekdayLongLabel,
  weekdayOf,
  weekOrder,
} from "@/features/classGroups/weekdays";
import {
  dayOfMonth,
  monthGridOf,
  monthOfDate,
  parseIsoDate,
  weekOf,
} from "@/features/sessions/dates";
import { CalendarDay } from "@/features/sessions/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

export type AgendaView = "day" | "month";

interface AgendaCalendarProps {
  view: AgendaView;
  selectedDate: string;
  today: string;
  month: string;
  monthDays: readonly CalendarDay[];
  hasClassesOn: (isoDate: string) => boolean;
  onSelectDate: (isoDate: string) => void;
  onPrevious: () => void;
  onNext: () => void;
}

const daysPerWeek = 7;
const labelSeparator = " · ";
const shortMonthLength = 3;

function shortMonthOf(isoDate: string): string {
  const monthNumber = parseIsoDate(isoDate).getMonth() + 1;
  return translate(`months.${monthNumber}` as TranslationKey).slice(0, shortMonthLength);
}

function weekRangeLabel(week: readonly string[]): string {
  const first = week[0]!;
  const last = week[week.length - 1]!;
  const firstMonth = monthOfDate(first) === monthOfDate(last) ? "" : ` ${shortMonthOf(first)}`;
  return `${dayOfMonth(first)}${firstMonth} – ${dayOfMonth(last)} ${shortMonthOf(last)}`;
}

function monthDayLabel(isoDate: string, calendarDay: CalendarDay | undefined): string {
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

interface DayCellProps {
  isoDate: string;
  accessibilityLabel: string;
  isSelected: boolean;
  isToday: boolean;
  isOutside: boolean;
  hasClasses: boolean;
  hasPendingAttendance: boolean;
  hasCancellation: boolean;
  onPress: () => void;
}

function DayCell({
  isoDate,
  accessibilityLabel,
  isSelected,
  isToday,
  isOutside,
  hasClasses,
  hasPendingAttendance,
  hasCancellation,
  onPress,
}: DayCellProps) {
  const backgroundClassName = isSelected
    ? "bg-primary"
    : hasCancellation
      ? "bg-danger-soft"
      : "bg-transparent";
  const dotClassName = hasPendingAttendance
    ? "bg-warning"
    : hasClasses
      ? isSelected
        ? "bg-primary-foreground"
        : "bg-primary"
      : "bg-transparent";
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityState={{ selected: isSelected }}
      onPress={onPress}
      className={`h-11 flex-1 items-center justify-center gap-[3px] rounded-xl border-[1.5px] ${isToday && !isSelected ? "border-primary" : "border-transparent"} ${backgroundClassName} ${isOutside ? "opacity-40" : ""}`}
    >
      <AppText
        variant="bodyStrong"
        tone={isSelected ? "onPrimary" : hasCancellation ? "dangerSoft" : "default"}
        className="font-strong text-base"
      >
        {dayOfMonth(isoDate)}
      </AppText>
      <View className={`h-1.5 w-1.5 rounded-full ${dotClassName}`} />
    </Pressable>
  );
}

export function AgendaCalendar({
  view,
  selectedDate,
  today,
  month,
  monthDays,
  hasClassesOn,
  onSelectDate,
  onPrevious,
  onNext,
}: AgendaCalendarProps) {
  const isMonth = view === "month";
  const calendarDays = new Map(monthDays.map((calendarDay) => [calendarDay.date, calendarDay]));
  const dates = isMonth ? monthGridOf(month) : weekOf(selectedDate);
  const weeks = Array.from({ length: dates.length / daysPerWeek }, (_, weekIndex) =>
    dates.slice(weekIndex * daysPerWeek, (weekIndex + 1) * daysPerWeek),
  );
  const previousLabel = translate(
    isMonth ? "sessions.calendar.previousMonth" : "sessions.calendar.previousWeek",
  );
  const nextLabel = translate(
    isMonth ? "sessions.calendar.nextMonth" : "sessions.calendar.nextWeek",
  );
  return (
    <Card className="gap-1 p-2">
      <View className="h-9 flex-row items-center gap-1">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={previousLabel}
          onPress={onPrevious}
          className="h-9 w-9 items-center justify-center rounded-full active:bg-muted"
        >
          <Icon name="previous" tone="primary" />
        </Pressable>
        <AppText variant="bodyStrong" tone="muted" className="flex-1 text-center">
          {isMonth ? translate("sessions.calendar.pickDay") : weekRangeLabel(dates)}
        </AppText>
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={nextLabel}
          onPress={onNext}
          className="h-9 w-9 items-center justify-center rounded-full active:bg-muted"
        >
          <Icon name="next" tone="primary" />
        </Pressable>
      </View>
      <View className="flex-row gap-1">
        {weekOrder.map((weekday) => (
          <AppText
            key={weekday}
            variant="footnote"
            tone="subtle"
            className="flex-1 pb-0.5 text-center font-label"
          >
            {weekdayLetter(weekday)}
          </AppText>
        ))}
      </View>
      {weeks.map((week) => (
        <View key={week[0]} className="flex-row gap-1">
          {week.map((isoDate) => {
            const calendarDay = calendarDays.get(isoDate);
            return (
              <DayCell
                key={isoDate}
                isoDate={isoDate}
                accessibilityLabel={
                  isMonth
                    ? monthDayLabel(isoDate, calendarDay)
                    : translate("sessions.day.pickDay", {
                        weekday: weekdayLongLabel(weekdayOf(parseIsoDate(isoDate))),
                        day: dayOfMonth(isoDate),
                      })
                }
                isSelected={isoDate === selectedDate}
                isToday={isoDate === today}
                isOutside={isMonth && monthOfDate(isoDate) !== month}
                hasClasses={isMonth ? (calendarDay?.classCount ?? 0) > 0 : hasClassesOn(isoDate)}
                hasPendingAttendance={isMonth && (calendarDay?.pendingAttendanceCount ?? 0) > 0}
                hasCancellation={isMonth && (calendarDay?.cancelledCount ?? 0) > 0}
                onPress={() => onSelectDate(isoDate)}
              />
            );
          })}
        </View>
      ))}
    </Card>
  );
}

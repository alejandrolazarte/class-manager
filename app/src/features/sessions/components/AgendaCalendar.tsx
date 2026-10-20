import { View } from "react-native";
import { weekdayLongLabel, weekdayOf, weekdayShortLabel, weekOrder } from "@/dates/weekdays";
import { dayOfMonth, monthGridOf, monthOfDate, parseIsoDate, weekOf } from "@/dates/dates";
import { CalendarDay } from "@/features/sessions/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { DayStrip } from "@/ui/DayStrip";
import { MonthCalendar, MonthDayMarker } from "@/ui/MonthCalendar";
import { StepArrow } from "@/ui/StepArrow";

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

function markerOf(calendarDay: CalendarDay | undefined): MonthDayMarker {
  if ((calendarDay?.pendingAttendanceCount ?? 0) > 0) {
    return "pending";
  }
  return (calendarDay?.classCount ?? 0) > 0 ? "classes" : "none";
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
  const week = weekOf(selectedDate);
  return (
    <View className="gap-1.5">
      <View className="flex-row items-center gap-1">
        <StepArrow
          direction="previous"
          label={translate(
            isMonth ? "sessions.calendar.previousMonth" : "sessions.calendar.previousWeek",
          )}
          onPress={onPrevious}
        />
        <AppText variant="caption" tone="subtle" className="flex-1 text-center">
          {isMonth ? translate("sessions.calendar.pickDay") : weekRangeLabel(week)}
        </AppText>
        <StepArrow
          direction="next"
          label={translate(isMonth ? "sessions.calendar.nextMonth" : "sessions.calendar.nextWeek")}
          onPress={onNext}
        />
      </View>
      {isMonth ? (
        <MonthCalendar
          weekdayLabels={weekOrder.map(weekdayShortLabel)}
          days={monthGridOf(month).map((isoDate) => {
            const calendarDay = calendarDays.get(isoDate);
            return {
              key: isoDate,
              dayNumber: dayOfMonth(isoDate),
              accessibilityLabel: monthDayLabel(isoDate, calendarDay),
              isSelected: isoDate === selectedDate,
              isToday: isoDate === today,
              isOutside: monthOfDate(isoDate) !== month,
              isCancelled: (calendarDay?.cancelledCount ?? 0) > 0,
              marker: markerOf(calendarDay),
            };
          })}
          onSelect={onSelectDate}
        />
      ) : (
        <DayStrip
          days={week.map((isoDate) => ({
            key: isoDate,
            label: weekdayShortLabel(weekdayOf(parseIsoDate(isoDate))),
            accessibilityLabel: translate("sessions.day.pickDay", {
              weekday: weekdayLongLabel(weekdayOf(parseIsoDate(isoDate))),
              day: dayOfMonth(isoDate),
            }),
            dayNumber: dayOfMonth(isoDate),
            hasClasses: hasClassesOn(isoDate),
            isToday: isoDate === today,
          }))}
          selectedKeys={[selectedDate]}
          onSelect={onSelectDate}
        />
      )}
    </View>
  );
}

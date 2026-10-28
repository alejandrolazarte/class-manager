import { View } from "react-native";
import { weekdayOf, weekdayShortLabel, weekOrder } from "@/dates/weekdays";
import { dayOfMonth, monthGridOf, monthOfDate, parseIsoDate, weekOf } from "@/dates/dates";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { DayMarker } from "@/ui/DayMarker";
import { DayStrip } from "@/ui/DayStrip";
import { MonthCalendar } from "@/ui/MonthCalendar";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { StepArrow } from "@/ui/StepArrow";

export type AgendaView = "day" | "month";

export interface AgendaWeekDay {
  marker: DayMarker;
  accessibilityLabel: string;
}

export interface AgendaMonthDay extends AgendaWeekDay {
  isCancelled: boolean;
}

interface AgendaCalendarProps {
  view: AgendaView;
  onChangeView: (view: AgendaView) => void;
  eyebrow: string;
  title: string;
  selectedDate: string;
  today: string;
  weekDayOf: (isoDate: string) => AgendaWeekDay;
  monthDayOf: (isoDate: string) => AgendaMonthDay;
  onSelectDate: (isoDate: string) => void;
  onPrevious: () => void;
  onNext: () => void;
  isPreviousDisabled?: boolean;
  isNextDisabled?: boolean;
}

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

export function AgendaCalendar({
  view,
  onChangeView,
  eyebrow,
  title,
  selectedDate,
  today,
  weekDayOf,
  monthDayOf,
  onSelectDate,
  onPrevious,
  onNext,
  isPreviousDisabled = false,
  isNextDisabled = false,
}: AgendaCalendarProps) {
  const isMonth = view === "month";
  const month = monthOfDate(selectedDate);
  const week = weekOf(selectedDate);
  return (
    <OutlinedBlock className="mt-1 gap-3">
      <View className="h-11 flex-row items-end justify-between gap-3">
        <View className="min-w-0 flex-1">
          <AppText variant="eyebrow" tone="subtle" numberOfLines={1}>
            {eyebrow}
          </AppText>
          <AppText variant="heading" accessibilityRole="header" numberOfLines={1}>
            {title}
          </AppText>
        </View>
        <SegmentedControl
          isCompact
          options={[
            { value: "day", label: translate("home.view.day") },
            { value: "month", label: translate("home.view.month") },
          ]}
          selectedValue={view}
          onChange={onChangeView}
        />
      </View>
      <View className="gap-1.5">
        <View className="flex-row items-center gap-1">
          <StepArrow
            direction="previous"
            label={translate(
              isMonth ? "sessions.calendar.previousMonth" : "sessions.calendar.previousWeek",
            )}
            onPress={onPrevious}
            disabled={isPreviousDisabled}
          />
          <AppText variant="caption" tone="subtle" className="flex-1 text-center">
            {isMonth ? translate("sessions.calendar.pickDay") : weekRangeLabel(week)}
          </AppText>
          <StepArrow
            direction="next"
            label={translate(
              isMonth ? "sessions.calendar.nextMonth" : "sessions.calendar.nextWeek",
            )}
            onPress={onNext}
            disabled={isNextDisabled}
          />
        </View>
        {isMonth ? (
          <MonthCalendar
            weekdayLabels={weekOrder.map(weekdayShortLabel)}
            days={monthGridOf(month).map((isoDate) => {
              const monthDay = monthDayOf(isoDate);
              return {
                key: isoDate,
                dayNumber: dayOfMonth(isoDate),
                accessibilityLabel: monthDay.accessibilityLabel,
                isSelected: isoDate === selectedDate,
                isToday: isoDate === today,
                isOutside: monthOfDate(isoDate) !== month,
                isCancelled: monthDay.isCancelled,
                marker: monthDay.marker,
              };
            })}
            onSelect={onSelectDate}
          />
        ) : (
          <DayStrip
            days={week.map((isoDate) => {
              const weekDay = weekDayOf(isoDate);
              return {
                key: isoDate,
                label: weekdayShortLabel(weekdayOf(parseIsoDate(isoDate))),
                accessibilityLabel: weekDay.accessibilityLabel,
                dayNumber: dayOfMonth(isoDate),
                marker: weekDay.marker,
                isToday: isoDate === today,
              };
            })}
            selectedKeys={[selectedDate]}
            onSelect={onSelectDate}
          />
        )}
      </View>
    </OutlinedBlock>
  );
}

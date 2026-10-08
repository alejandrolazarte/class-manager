import { useState } from "react";
import { Pressable, View } from "react-native";
import { weekdayShortLabel, weekOrder } from "@/features/classGroups/weekdays";
import { monthGridOf, monthOfDate, parseIsoDate, todayIsoDate } from "@/features/sessions/dates";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface CalendarPickerProps {
  selectedIsoDate: string | null;
  onSelect: (isoDate: string) => void;
  earliestIsoDate: string;
  latestIsoDate: string;
}

type CalendarView = "days" | "years";

const monthsPerYear = 12;
const isoMonthLength = 7;
const isoYearLength = 4;

function shiftMonth(month: string, monthCount: number): string {
  const [year, monthNumber] = month.split("-").map(Number);
  const monthIndex = (year ?? 0) * monthsPerYear + (monthNumber ?? 1) - 1 + monthCount;
  const shiftedYear = Math.floor(monthIndex / monthsPerYear);
  const shiftedMonthNumber = (monthIndex % monthsPerYear) + 1;
  return `${shiftedYear}-${String(shiftedMonthNumber).padStart(2, "0")}`;
}

function clampMonth(month: string, earliestMonth: string, latestMonth: string): string {
  if (month < earliestMonth) {
    return earliestMonth;
  }
  return month > latestMonth ? latestMonth : month;
}

function monthTitle(month: string): string {
  const [year, monthNumber] = month.split("-");
  const monthName = translate(`months.${Number(monthNumber)}` as TranslationKey);
  return translate("dateField.monthTitle", {
    month: monthName.charAt(0).toUpperCase() + monthName.slice(1),
    year: year ?? "",
  });
}

function longDayLabel(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  return `${date.getDate()} ${translate(`months.${date.getMonth() + 1}` as TranslationKey)} ${date.getFullYear()}`;
}

export function CalendarPicker({
  selectedIsoDate,
  onSelect,
  earliestIsoDate,
  latestIsoDate,
}: CalendarPickerProps) {
  const today = todayIsoDate();
  const earliestMonth = monthOfDate(earliestIsoDate);
  const latestMonth = monthOfDate(latestIsoDate);
  const [visibleMonth, setVisibleMonth] = useState(() =>
    clampMonth(monthOfDate(selectedIsoDate ?? today), earliestMonth, latestMonth),
  );
  const [view, setView] = useState<CalendarView>("days");

  if (view === "years") {
    const earliestYear = Number(earliestIsoDate.slice(0, isoYearLength));
    const latestYear = Number(latestIsoDate.slice(0, isoYearLength));
    const visibleYear = Number(visibleMonth.slice(0, isoYearLength));
    const years = Array.from(
      { length: latestYear - earliestYear + 1 },
      (_, yearIndex) => latestYear - yearIndex,
    );
    return (
      <View className="gap-2">
        <AppText variant="heading" className="text-center">
          {translate("dateField.chooseYear")}
        </AppText>
        <View className="flex-row flex-wrap">
          {years.map((year) => {
            const isSelected = year === visibleYear;
            return (
              <View key={year} className="w-1/4 p-1">
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={String(year)}
                  accessibilityState={{ selected: isSelected }}
                  onPress={() => {
                    const month = visibleMonth.slice(isoYearLength);
                    setVisibleMonth(clampMonth(`${year}${month}`, earliestMonth, latestMonth));
                    setView("days");
                  }}
                  className={`h-11 items-center justify-center rounded-xl ${isSelected ? "bg-primary" : "bg-muted active:bg-border"}`}
                >
                  <AppText variant="bodyStrong" tone={isSelected ? "onPrimary" : "default"}>
                    {year}
                  </AppText>
                </Pressable>
              </View>
            );
          })}
        </View>
      </View>
    );
  }

  return (
    <View className="gap-1.5">
      <View className="flex-row items-center justify-between">
        <IconButton
          icon="previous"
          accessibilityLabel={translate("dateField.previousMonth")}
          disabled={visibleMonth <= earliestMonth}
          onPress={() => setVisibleMonth(shiftMonth(visibleMonth, -1))}
        />
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("dateField.chooseYear")}
          accessibilityHint={monthTitle(visibleMonth)}
          onPress={() => setView("years")}
          className="rounded-xl px-3 py-2 active:bg-muted"
        >
          <AppText variant="heading">{monthTitle(visibleMonth)}</AppText>
        </Pressable>
        <IconButton
          icon="next"
          accessibilityLabel={translate("dateField.nextMonth")}
          disabled={visibleMonth >= latestMonth}
          onPress={() => setVisibleMonth(shiftMonth(visibleMonth, 1))}
        />
      </View>
      <View className="flex-row">
        {weekOrder.map((weekday) => (
          <AppText key={weekday} variant="label" tone="subtle" className="flex-1 py-1 text-center">
            {weekdayShortLabel(weekday)}
          </AppText>
        ))}
      </View>
      <View className="flex-row flex-wrap">
        {monthGridOf(visibleMonth).map((isoDate) => {
          const isInMonth = isoDate.slice(0, isoMonthLength) === visibleMonth;
          const isSelectable = isInMonth && isoDate >= earliestIsoDate && isoDate <= latestIsoDate;
          const isSelected = isoDate === selectedIsoDate;
          const isToday = isoDate === today;
          return (
            <View key={isoDate} className="w-[14.2857%] p-0.5">
              {isInMonth ? (
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={longDayLabel(isoDate)}
                  accessibilityState={{ selected: isSelected, disabled: !isSelectable }}
                  disabled={!isSelectable}
                  onPress={() => onSelect(isoDate)}
                  className={`h-11 items-center justify-center rounded-full ${isSelected ? "bg-primary" : isToday ? "border-[1.5px] border-primary" : "active:bg-muted"}`}
                >
                  <AppText
                    variant={isSelected || isToday ? "bodyStrong" : "body"}
                    tone={isSelected ? "onPrimary" : isSelectable ? "default" : "disabled"}
                  >
                    {parseIsoDate(isoDate).getDate()}
                  </AppText>
                </Pressable>
              ) : (
                <View className="h-11" />
              )}
            </View>
          );
        })}
      </View>
    </View>
  );
}

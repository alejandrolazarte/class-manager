import { useRef, useState } from "react";
import { Pressable, ScrollView, View } from "react-native";
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

type CalendarView = "days" | "years" | "months";

const monthsPerYear = 12;
const isoMonthLength = 7;
const isoYearLength = 4;
const yearsPerRow = 4;
const yearRowHeight = 52;
const yearListHeight = 288;

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

function yearOf(isoDateOrMonth: string): number {
  return Number(isoDateOrMonth.slice(0, isoYearLength));
}

function monthName(monthNumber: number): string {
  return translate(`months.${monthNumber}` as TranslationKey);
}

function capitalized(text: string): string {
  return text.charAt(0).toUpperCase() + text.slice(1);
}

function monthTitle(month: string): string {
  const [year, monthNumber] = month.split("-");
  return translate("dateField.monthTitle", {
    month: capitalized(monthName(Number(monthNumber))),
    year: year ?? "",
  });
}

interface YearListProps {
  earliestYear: number;
  latestYear: number;
  selectedYear: number;
  onSelect: (year: number) => void;
}

function YearList({ earliestYear, latestYear, selectedYear, onSelect }: YearListProps) {
  const listRef = useRef<ScrollView>(null);
  const years = Array.from(
    { length: latestYear - earliestYear + 1 },
    (_, yearIndex) => latestYear - yearIndex,
  );
  const yearRows = Array.from({ length: Math.ceil(years.length / yearsPerRow) }, (_, rowIndex) =>
    years.slice(rowIndex * yearsPerRow, (rowIndex + 1) * yearsPerRow),
  );
  const scrollToSelectedYear = () => {
    const selectedRowIndex = Math.floor(years.indexOf(selectedYear) / yearsPerRow);
    const rowTop = Math.max(0, selectedRowIndex) * yearRowHeight;
    listRef.current?.scrollTo({
      y: Math.max(0, rowTop - (yearListHeight - yearRowHeight) / 2),
      animated: false,
    });
  };
  return (
    <View className="gap-2">
      <AppText variant="heading" className="text-center">
        {translate("dateField.chooseYear")}
      </AppText>
      <ScrollView
        ref={listRef}
        nestedScrollEnabled
        onLayout={scrollToSelectedYear}
        className="h-72 rounded-2xl bg-surface"
        contentContainerClassName="p-1"
      >
        {yearRows.map((yearRow) => (
          <View key={yearRow[0]} className="flex-row">
            {yearRow.map((year) => {
              const isSelected = year === selectedYear;
              return (
                <View key={year} className="w-1/4 p-1">
                  <Pressable
                    accessibilityRole="button"
                    accessibilityLabel={String(year)}
                    accessibilityState={{ selected: isSelected }}
                    onPress={() => onSelect(year)}
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
        ))}
      </ScrollView>
    </View>
  );
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
    return (
      <YearList
        earliestYear={yearOf(earliestIsoDate)}
        latestYear={yearOf(latestIsoDate)}
        selectedYear={yearOf(visibleMonth)}
        onSelect={(year) => {
          setVisibleMonth(`${year}${visibleMonth.slice(isoYearLength)}`);
          setView("months");
        }}
      />
    );
  }

  if (view === "months") {
    const visibleYear = yearOf(visibleMonth);
    return (
      <View className="gap-2">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("dateField.chooseYear")}
          accessibilityHint={String(visibleYear)}
          onPress={() => setView("years")}
          className="self-center rounded-xl px-3 py-2 active:bg-muted"
        >
          <AppText variant="heading">{visibleYear}</AppText>
        </Pressable>
        <View className="flex-row flex-wrap">
          {Array.from({ length: monthsPerYear }, (_, monthIndex) => {
            const month = `${visibleYear}-${String(monthIndex + 1).padStart(2, "0")}`;
            const isSelectable = month >= earliestMonth && month <= latestMonth;
            const isSelected = month === visibleMonth;
            return (
              <View key={month} className="w-1/3 p-1">
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={monthTitle(month)}
                  accessibilityState={{ selected: isSelected, disabled: !isSelectable }}
                  disabled={!isSelectable}
                  onPress={() => {
                    setVisibleMonth(month);
                    setView("days");
                  }}
                  className={`h-12 items-center justify-center rounded-xl ${isSelected ? "bg-primary" : "bg-muted active:bg-border"}`}
                >
                  <AppText
                    variant="bodyStrong"
                    tone={isSelected ? "onPrimary" : isSelectable ? "default" : "disabled"}
                  >
                    {capitalized(monthName(monthIndex + 1))}
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

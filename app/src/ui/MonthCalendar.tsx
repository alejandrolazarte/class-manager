import { Pressable, View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";

export type MonthDayMarker = "none" | "classes" | "pending";

export interface MonthCalendarDay {
  key: string;
  dayNumber: number;
  accessibilityLabel: string;
  isSelected: boolean;
  isToday: boolean;
  isOutside: boolean;
  isCancelled: boolean;
  marker: MonthDayMarker;
}

interface MonthCalendarProps {
  weekdayLabels: readonly string[];
  days: readonly MonthCalendarDay[];
  onSelect: (key: string) => void;
}

const daysPerWeek = 7;

const markerClassNames: Record<MonthDayMarker, string> = {
  none: "bg-transparent",
  classes: "bg-subtle-foreground",
  pending: "bg-warning",
};

function circleClassName(day: MonthCalendarDay): string {
  if (day.isSelected) {
    return "bg-primary";
  }
  if (day.isCancelled) {
    return "bg-danger-soft";
  }
  return day.isToday ? "border-[1.5px] border-primary" : "";
}

function numberTone(day: MonthCalendarDay): TextTone {
  if (day.isSelected) {
    return "onPrimary";
  }
  if (day.isCancelled) {
    return "dangerSoft";
  }
  return day.isToday ? "primary" : "default";
}

export function MonthCalendar({ weekdayLabels, days, onSelect }: MonthCalendarProps) {
  const weeks = Array.from({ length: Math.ceil(days.length / daysPerWeek) }, (_, weekIndex) =>
    days.slice(weekIndex * daysPerWeek, (weekIndex + 1) * daysPerWeek),
  );
  return (
    <View>
      <View className="flex-row pb-1">
        {weekdayLabels.map((weekdayLabel) => (
          <AppText key={weekdayLabel} variant="tab" tone="subtle" className="flex-1 text-center">
            {weekdayLabel}
          </AppText>
        ))}
      </View>
      {weeks.map((week) => (
        <View key={week[0]?.key} className="flex-row">
          {week.map((day) => (
            <Pressable
              key={day.key}
              accessibilityRole="button"
              accessibilityLabel={day.accessibilityLabel}
              accessibilityState={{ selected: day.isSelected }}
              onPress={() => onSelect(day.key)}
              className={`h-[38px] flex-1 items-center justify-center gap-0.5 ${day.isOutside ? "opacity-40" : ""}`}
            >
              <View
                className={`h-7 w-7 items-center justify-center rounded-full ${circleClassName(day)}`}
              >
                <AppText
                  variant={day.isSelected || day.isToday ? "bodyStrong" : "body"}
                  tone={numberTone(day)}
                >
                  {day.dayNumber}
                </AppText>
              </View>
              <View className={`h-1 w-1 rounded-full ${markerClassNames[day.marker]}`} />
            </Pressable>
          ))}
        </View>
      ))}
    </View>
  );
}

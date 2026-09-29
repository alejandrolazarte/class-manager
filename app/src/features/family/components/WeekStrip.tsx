import { Pressable, View } from "react-native";
import { weekdayShortLabel, weekdayOf } from "@/features/classGroups/weekdays";
import { shortDayLabel } from "@/features/family/familySchedule";
import { dayOfMonth, parseIsoDate } from "@/features/sessions/dates";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

interface WeekStripProps {
  days: readonly string[];
  today: string;
  selectedDate: string;
  hasClassesOn: (isoDate: string) => boolean;
  onSelect: (isoDate: string) => void;
}

export function WeekStrip({ days, today, selectedDate, hasClassesOn, onSelect }: WeekStripProps) {
  return (
    <Card className="flex-row gap-1 p-2">
      {days.map((isoDate) => {
        const isSelected = isoDate === selectedDate;
        const dotClassName = hasClassesOn(isoDate)
          ? isSelected
            ? "bg-primary-foreground"
            : "bg-primary"
          : "bg-transparent";
        return (
          <Pressable
            key={isoDate}
            accessibilityRole="button"
            accessibilityLabel={shortDayLabel(isoDate)}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(isoDate)}
            className={`flex-1 items-center gap-[3px] rounded-[14px] py-2 ${isSelected ? "bg-primary" : isoDate === today ? "bg-muted" : "active:bg-muted"}`}
          >
            <AppText variant="label" tone={isSelected ? "onPrimary" : "subtle"}>
              {weekdayShortLabel(weekdayOf(parseIsoDate(isoDate)))}
            </AppText>
            <AppText
              variant="heading"
              tone={isSelected ? "onPrimary" : "default"}
              className="font-heavy"
            >
              {dayOfMonth(isoDate)}
            </AppText>
            <View className={`h-1.5 w-1.5 rounded-full ${dotClassName}`} />
          </Pressable>
        );
      })}
    </Card>
  );
}

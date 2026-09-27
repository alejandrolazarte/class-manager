import { Pressable, View } from "react-native";
import { weekdayLongLabel, weekdayOf, weekdayShortLabel } from "@/features/classGroups/weekdays";
import { dayOfMonth, parseIsoDate, weekOf } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";

interface WeekStripProps {
  selectedDate: string;
  today: string;
  onSelectDate: (isoDate: string) => void;
}

export function WeekStrip({ selectedDate, today, onSelectDate }: WeekStripProps) {
  const elevationStyle = useElevationStyle();
  return (
    <View className="flex-row gap-1.5">
      {weekOf(selectedDate).map((isoDate) => {
        const isSelected = isoDate === selectedDate;
        const weekday = weekdayOf(parseIsoDate(isoDate));
        return (
          <Pressable
            key={isoDate}
            accessibilityRole="button"
            accessibilityLabel={translate("sessions.day.pickDay", {
              weekday: weekdayLongLabel(weekday),
              day: dayOfMonth(isoDate),
            })}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelectDate(isoDate)}
            style={isSelected ? undefined : elevationStyle}
            className={`flex-1 items-center gap-[3px] rounded-2xl pb-[9px] pt-2 ${isSelected ? "bg-primary" : "bg-surface"}`}
          >
            <AppText
              variant="footnote"
              tone={isSelected ? "onPrimary" : "subtle"}
              className="font-label"
            >
              {weekdayShortLabel(weekday)}
            </AppText>
            <AppText
              variant="headline"
              tone={isSelected ? "onPrimary" : "default"}
              className="text-lg leading-5"
            >
              {dayOfMonth(isoDate)}
            </AppText>
            <View
              className={`h-[5px] w-[5px] rounded-full ${isoDate === today ? (isSelected ? "bg-primary-foreground" : "bg-primary") : "bg-transparent"}`}
            />
          </Pressable>
        );
      })}
    </View>
  );
}

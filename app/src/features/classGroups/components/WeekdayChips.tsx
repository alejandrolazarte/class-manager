import { View } from "react-native";
import { Weekday } from "@/features/classGroups/types";
import { weekdayLongLabel, weekdayShortLabel, weekOrder } from "@/features/classGroups/weekdays";
import { Chip } from "@/ui/Chip";

interface WeekdayChipsProps {
  selectedWeekdays: readonly Weekday[];
  onToggleWeekday: (weekday: Weekday) => void;
}

export function WeekdayChips({ selectedWeekdays, onToggleWeekday }: WeekdayChipsProps) {
  return (
    <View className="flex-row gap-1.5">
      {weekOrder.map((weekday) => (
        <Chip
          key={weekday}
          label={weekdayShortLabel(weekday)}
          accessibilityLabel={weekdayLongLabel(weekday)}
          isSelected={selectedWeekdays.includes(weekday)}
          onPress={() => onToggleWeekday(weekday)}
          shape="tile"
          className="flex-1"
        />
      ))}
    </View>
  );
}

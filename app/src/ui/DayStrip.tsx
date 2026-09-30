import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

export interface DayStripDay {
  key: string;
  label: string;
  accessibilityLabel: string;
  dayNumber?: number;
  hasClasses?: boolean;
  isToday?: boolean;
}

interface DayStripProps {
  days: readonly DayStripDay[];
  selectedKeys: readonly string[];
  onSelect: (key: string) => void;
  showsClassDots?: boolean;
}

function dotClassName(hasClasses: boolean, isSelected: boolean): string {
  if (!hasClasses) {
    return "bg-transparent";
  }
  return isSelected ? "bg-primary-foreground" : "bg-primary";
}

export function DayStrip({ days, selectedKeys, onSelect, showsClassDots = true }: DayStripProps) {
  return (
    <Card className="flex-row gap-1 p-2">
      {days.map((day) => {
        const isSelected = selectedKeys.includes(day.key);
        const hasDayNumber = day.dayNumber !== undefined;
        return (
          <Pressable
            key={day.key}
            accessibilityRole="button"
            accessibilityLabel={day.accessibilityLabel}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(day.key)}
            className={`flex-1 items-center gap-[3px] rounded-[14px] py-2 ${isSelected ? "bg-primary" : day.isToday ? "bg-muted" : "active:bg-muted"}`}
          >
            {hasDayNumber ? (
              <>
                <AppText variant="label" tone={isSelected ? "onPrimary" : "subtle"}>
                  {day.label}
                </AppText>
                <AppText
                  variant="heading"
                  tone={isSelected ? "onPrimary" : "default"}
                  className="font-heavy"
                >
                  {day.dayNumber}
                </AppText>
              </>
            ) : (
              <AppText
                variant="heading"
                tone={isSelected ? "onPrimary" : "default"}
                className="font-heavy"
              >
                {day.label}
              </AppText>
            )}
            {showsClassDots ? (
              <View
                className={`h-1.5 w-1.5 rounded-full ${dotClassName(day.hasClasses ?? false, isSelected)}`}
              />
            ) : null}
          </Pressable>
        );
      })}
    </Card>
  );
}

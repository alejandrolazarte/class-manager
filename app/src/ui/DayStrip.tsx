import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

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
  const { layouts, onOptionLayout } = useOptionLayouts<string>();
  const slidingKey = selectedKeys.length === 1 ? selectedKeys[0] : undefined;
  return (
    <Card className="p-2">
      <View className="relative flex-row gap-1">
        <SlidingIndicator
          layout={slidingKey === undefined ? undefined : layouts[slidingKey]}
          className="rounded-[14px] bg-primary"
        />
        {days.map((day) => {
          const isSelected = selectedKeys.includes(day.key);
          const selectedBackground = slidingKey === undefined ? "bg-primary" : "";
          const hasDayNumber = day.dayNumber !== undefined;
          return (
            <Pressable
              key={day.key}
              accessibilityRole="button"
              accessibilityLabel={day.accessibilityLabel}
              accessibilityState={{ selected: isSelected }}
              onPress={() => onSelect(day.key)}
              onLayout={onOptionLayout(day.key)}
              className={`flex-1 items-center gap-[3px] rounded-[14px] py-2 ${isSelected ? selectedBackground : day.isToday ? "bg-muted" : "active:bg-muted"}`}
            >
              {hasDayNumber ? (
                <>
                  <AppText variant="label" tone={isSelected ? "onPrimary" : "subtle"}>
                    {day.label}
                  </AppText>
                  <AppText variant="heading" tone={isSelected ? "onPrimary" : "default"}>
                    {day.dayNumber}
                  </AppText>
                </>
              ) : (
                <AppText variant="heading" tone={isSelected ? "onPrimary" : "default"}>
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
      </View>
    </Card>
  );
}

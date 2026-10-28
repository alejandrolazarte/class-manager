import { LayoutRectangle, Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { DayMarker, dayMarkerClassNames } from "@/ui/DayMarker";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

export interface DayStripDay {
  key: string;
  label: string;
  accessibilityLabel: string;
  dayNumber?: number;
  marker?: DayMarker;
  isToday?: boolean;
}

interface DayStripProps {
  days: readonly DayStripDay[];
  selectedKeys: readonly string[];
  onSelect: (key: string) => void;
  showsClassDots?: boolean;
}

const circleSize = 36;
const labelRowHeight = 16;
const classDotTestIdPrefix = "day-strip-class-dot-";

function circleLayoutOf(
  cellLayout: LayoutRectangle | undefined,
  hasDayNumbers: boolean,
): LayoutRectangle | undefined {
  if (cellLayout === undefined) {
    return undefined;
  }
  return {
    x: cellLayout.x + (cellLayout.width - circleSize) / 2,
    y: cellLayout.y + (hasDayNumbers ? labelRowHeight : 0),
    width: circleSize,
    height: circleSize,
  };
}

function dotClassName(marker: DayMarker, isSelected: boolean): string {
  if (marker === "none") {
    return dayMarkerClassNames.none;
  }
  return isSelected ? "bg-primary" : dayMarkerClassNames[marker];
}

export function DayStrip({ days, selectedKeys, onSelect, showsClassDots = true }: DayStripProps) {
  const { layouts, onOptionLayout } = useOptionLayouts<string>();
  const slidingKey = selectedKeys.length === 1 ? selectedKeys[0] : undefined;
  const hasDayNumbers = days.some((day) => day.dayNumber !== undefined);
  return (
    <View className="relative flex-row">
      <SlidingIndicator
        layout={
          slidingKey === undefined ? undefined : circleLayoutOf(layouts[slidingKey], hasDayNumbers)
        }
        className="rounded-full bg-primary"
      />
      {days.map((day) => {
        const isSelected = selectedKeys.includes(day.key);
        const circleBackground = isSelected && slidingKey === undefined ? "bg-primary" : "";
        const circleText = day.dayNumber === undefined ? day.label : String(day.dayNumber);
        const marker = day.marker ?? "none";
        return (
          <Pressable
            key={day.key}
            accessibilityRole="button"
            accessibilityLabel={day.accessibilityLabel}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(day.key)}
            onLayout={onOptionLayout(day.key)}
            className="flex-1 items-center"
          >
            {hasDayNumbers ? (
              <AppText variant="tab" tone="subtle" className="h-4">
                {day.label}
              </AppText>
            ) : null}
            <View
              className={`h-9 w-9 items-center justify-center rounded-full ${circleBackground}`}
            >
              <AppText
                variant={isSelected || day.isToday ? "bodyStrong" : "body"}
                tone={isSelected ? "onPrimary" : day.isToday ? "primary" : "default"}
              >
                {circleText}
              </AppText>
            </View>
            {showsClassDots ? (
              <View
                testID={marker === "none" ? undefined : `${classDotTestIdPrefix}${day.key}`}
                className={`mt-0.5 h-1 w-1 rounded-full ${dotClassName(marker, isSelected)}`}
              />
            ) : null}
          </Pressable>
        );
      })}
    </View>
  );
}

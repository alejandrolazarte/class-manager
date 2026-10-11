import { LayoutRectangle, Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

export interface SegmentedOption<TValue extends string> {
  value: TValue;
  label: string;
  accessibilityLabel?: string;
}

interface SegmentedControlProps<TValue extends string> {
  options: readonly SegmentedOption<TValue>[];
  selectedValue: TValue;
  onChange: (value: TValue) => void;
  isCompact?: boolean;
}

const compactLineWidth = 20;
const compactLineHeight = 1.5;
const compactLineBottomGap = 3;

function compactLineLayoutOf(
  optionLayout: LayoutRectangle | undefined,
): LayoutRectangle | undefined {
  if (optionLayout === undefined) {
    return undefined;
  }
  return {
    x: optionLayout.x + (optionLayout.width - compactLineWidth) / 2,
    y: optionLayout.y + optionLayout.height - compactLineBottomGap - compactLineHeight,
    width: compactLineWidth,
    height: compactLineHeight,
  };
}

export function SegmentedControl<TValue extends string>({
  options,
  selectedValue,
  onChange,
  isCompact = false,
}: SegmentedControlProps<TValue>) {
  const { layouts, onOptionLayout } = useOptionLayouts<TValue>();
  return (
    <View
      accessibilityRole="tablist"
      className={`relative flex-row ${isCompact ? "" : "gap-1 rounded-xl bg-muted p-[3px]"}`}
    >
      <SlidingIndicator
        layout={isCompact ? compactLineLayoutOf(layouts[selectedValue]) : layouts[selectedValue]}
        className={isCompact ? "rounded-full bg-foreground" : "rounded-[9px] bg-surface"}
      />
      {options.map((option) => {
        const isSelected = option.value === selectedValue;
        return (
          <Pressable
            key={option.value}
            accessibilityRole="button"
            accessibilityLabel={option.accessibilityLabel ?? option.label}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onChange(option.value)}
            onLayout={onOptionLayout(option.value)}
            className={`items-center justify-center ${isCompact ? "h-8 px-2.5" : "h-[34px] flex-1 rounded-[9px] px-3"}`}
          >
            <AppText
              variant={isCompact ? "eyebrow" : "link"}
              tone={isSelected ? "default" : "muted"}
              numberOfLines={1}
            >
              {option.label}
            </AppText>
          </Pressable>
        );
      })}
    </View>
  );
}

import { Pressable, View } from "react-native";
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
      className={`relative flex-row gap-1 rounded-full bg-muted ${isCompact ? "p-[3px]" : "p-1"}`}
    >
      <SlidingIndicator layout={layouts[selectedValue]} className="rounded-full bg-surface" />
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
            className={`items-center justify-center rounded-full ${isCompact ? "h-[34px] px-3.5" : "h-10 flex-1 px-3"}`}
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

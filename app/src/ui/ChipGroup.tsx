import { ScrollView, View } from "react-native";
import { Chip } from "@/ui/Chip";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

export interface ChipGroupOption<TValue extends string> {
  value: TValue;
  label: string;
  accessibilityLabel?: string;
  count?: number;
}

interface ChipGroupProps<TValue extends string> {
  options: readonly ChipGroupOption<TValue>[];
  selectedValue: TValue;
  onChange: (value: TValue) => void;
  isScrollable?: boolean;
}

export function ChipGroup<TValue extends string>({
  options,
  selectedValue,
  onChange,
  isScrollable = false,
}: ChipGroupProps<TValue>) {
  const { layouts, onOptionLayout } = useOptionLayouts<TValue>();
  const content = (
    <>
      <SlidingIndicator layout={layouts[selectedValue]} className="rounded-full bg-inverse" />
      {options.map((option) => (
        <Chip
          key={option.value}
          label={option.label}
          accessibilityLabel={option.accessibilityLabel}
          count={option.count}
          isSelected={option.value === selectedValue}
          onPress={() => onChange(option.value)}
          onLayout={onOptionLayout(option.value)}
          hasSlidingSelection
        />
      ))}
    </>
  );
  if (isScrollable) {
    return (
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        className="-mx-5"
        contentContainerClassName="relative gap-1 px-5"
      >
        {content}
      </ScrollView>
    );
  }
  return <View className="relative flex-row flex-wrap gap-1">{content}</View>;
}

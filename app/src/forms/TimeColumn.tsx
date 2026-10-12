import { useRef } from "react";
import { Pressable, ScrollView, View } from "react-native";
import { AppText } from "@/ui/AppText";

const optionHeight = 44;
const optionGap = 4;
const columnHeight = 320;

export function twoDigits(timePart: number): string {
  return String(timePart).padStart(2, "0");
}

function centeredScrollOffset(optionIndex: number): number {
  const optionTop = optionIndex * (optionHeight + optionGap);
  return Math.max(0, optionTop - (columnHeight - optionHeight) / 2);
}

interface TimeColumnProps {
  title: string;
  options: readonly number[];
  selectedOption: number | null;
  initiallyVisibleOption: number;
  onSelect: (option: number) => void;
}

export function TimeColumn({
  title,
  options,
  selectedOption,
  initiallyVisibleOption,
  onSelect,
}: TimeColumnProps) {
  const columnRef = useRef<ScrollView>(null);
  const scrollToInitialOption = () => {
    const initialIndex = Math.max(0, options.indexOf(selectedOption ?? initiallyVisibleOption));
    columnRef.current?.scrollTo({ y: centeredScrollOffset(initialIndex), animated: false });
  };
  return (
    <View className="flex-1 gap-1.5">
      <AppText variant="label" tone="muted" className="text-center">
        {title}
      </AppText>
      <ScrollView
        ref={columnRef}
        nestedScrollEnabled
        showsVerticalScrollIndicator={false}
        onLayout={scrollToInitialOption}
        className="h-80 rounded-2xl bg-surface"
        contentContainerClassName="gap-1 p-1"
      >
        {options.map((option) => {
          const isSelected = option === selectedOption;
          return (
            <Pressable
              key={option}
              accessibilityRole="button"
              accessibilityLabel={`${title} ${twoDigits(option)}`}
              accessibilityState={{ selected: isSelected }}
              onPress={() => onSelect(option)}
              className={`h-11 items-center justify-center rounded-xl ${isSelected ? "bg-primary" : "bg-muted active:bg-border"}`}
            >
              <AppText variant="heading" tone={isSelected ? "onPrimary" : "default"}>
                {twoDigits(option)}
              </AppText>
            </Pressable>
          );
        })}
      </ScrollView>
    </View>
  );
}

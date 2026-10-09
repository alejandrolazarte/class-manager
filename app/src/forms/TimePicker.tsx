import { useRef } from "react";
import { Pressable, ScrollView, View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface TimePickerProps {
  selectedHour: number | null;
  selectedMinute: number | null;
  onSelectHour: (hour: number) => void;
  onSelectMinute: (minute: number) => void;
  minuteStep: number;
}

const hoursPerDay = 24;
const minutesPerHour = 60;
const optionHeight = 44;
const optionGap = 4;
const columnHeight = 320;
const defaultHour = 9;

function twoDigits(timePart: number): string {
  return String(timePart).padStart(2, "0");
}

function minuteOptions(minuteStep: number, selectedMinute: number | null): number[] {
  const steppedMinutes = Array.from(
    { length: Math.ceil(minutesPerHour / minuteStep) },
    (_, stepIndex) => stepIndex * minuteStep,
  );
  return selectedMinute === null || steppedMinutes.includes(selectedMinute)
    ? steppedMinutes
    : [...steppedMinutes, selectedMinute].sort((first, second) => first - second);
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

function TimeColumn({
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

const hourOptions = Array.from({ length: hoursPerDay }, (_, hour) => hour);

export function TimePicker({
  selectedHour,
  selectedMinute,
  onSelectHour,
  onSelectMinute,
  minuteStep,
}: TimePickerProps) {
  return (
    <View className="flex-row gap-3">
      <TimeColumn
        title={translate("timeField.hours")}
        options={hourOptions}
        selectedOption={selectedHour}
        initiallyVisibleOption={defaultHour}
        onSelect={onSelectHour}
      />
      <TimeColumn
        title={translate("timeField.minutes")}
        options={minuteOptions(minuteStep, selectedMinute)}
        selectedOption={selectedMinute}
        initiallyVisibleOption={0}
        onSelect={onSelectMinute}
      />
    </View>
  );
}

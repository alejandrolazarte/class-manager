import { Pressable, View } from "react-native";
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

interface TimeOptionProps {
  label: string;
  accessibilityLabel: string;
  isSelected: boolean;
  onPress: () => void;
}

function TimeOption({ label, accessibilityLabel, isSelected, onPress }: TimeOptionProps) {
  return (
    <View className="w-1/4 p-1">
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={accessibilityLabel}
        accessibilityState={{ selected: isSelected }}
        onPress={onPress}
        className={`h-11 items-center justify-center rounded-xl ${isSelected ? "bg-primary" : "bg-muted active:bg-border"}`}
      >
        <AppText variant="heading" tone={isSelected ? "onPrimary" : "default"}>
          {label}
        </AppText>
      </Pressable>
    </View>
  );
}

export function TimePicker({
  selectedHour,
  selectedMinute,
  onSelectHour,
  onSelectMinute,
  minuteStep,
}: TimePickerProps) {
  const hoursLabel = translate("timeField.hours");
  const minutesLabel = translate("timeField.minutes");
  return (
    <View className="gap-3">
      <View className="gap-1">
        <AppText variant="label" tone="muted">
          {hoursLabel}
        </AppText>
        <View className="flex-row flex-wrap">
          {Array.from({ length: hoursPerDay }, (_, hour) => (
            <TimeOption
              key={hour}
              label={twoDigits(hour)}
              accessibilityLabel={`${hoursLabel} ${twoDigits(hour)}`}
              isSelected={hour === selectedHour}
              onPress={() => onSelectHour(hour)}
            />
          ))}
        </View>
      </View>
      <View className="gap-1">
        <AppText variant="label" tone="muted">
          {minutesLabel}
        </AppText>
        <View className="flex-row flex-wrap">
          {minuteOptions(minuteStep, selectedMinute).map((minute) => (
            <TimeOption
              key={minute}
              label={twoDigits(minute)}
              accessibilityLabel={`${minutesLabel} ${twoDigits(minute)}`}
              isSelected={minute === selectedMinute}
              onPress={() => onSelectMinute(minute)}
            />
          ))}
        </View>
      </View>
    </View>
  );
}

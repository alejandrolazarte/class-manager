import { View } from "react-native";
import { TimeColumn } from "@/forms/TimeColumn";
import { translate } from "@/i18n/translate";

interface TimePickerProps {
  selectedHour: number | null;
  selectedMinute: number | null;
  onSelectHour: (hour: number) => void;
  onSelectMinute: (minute: number) => void;
  minuteStep: number;
}

const hoursPerDay = 24;
const minutesPerHour = 60;
const defaultHour = 9;

function minuteOptions(minuteStep: number, selectedMinute: number | null): number[] {
  const steppedMinutes = Array.from(
    { length: Math.ceil(minutesPerHour / minuteStep) },
    (_, stepIndex) => stepIndex * minuteStep,
  );
  return selectedMinute === null || steppedMinutes.includes(selectedMinute)
    ? steppedMinutes
    : [...steppedMinutes, selectedMinute].sort((first, second) => first - second);
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

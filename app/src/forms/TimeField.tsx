import { useState } from "react";
import { PickerSheet } from "@/forms/PickerSheet";
import { TimePicker } from "@/forms/TimePicker";
import {
  formatStartTimeAsTyped,
  startTimePattern,
} from "@/features/classGroups/startTimeFormatting";
import { translate } from "@/i18n/translate";
import { IconButton } from "@/ui/IconButton";
import { TextField, TextFieldSurface } from "@/ui/TextField";

interface TimeFieldProps {
  label: string;
  hint?: string;
  value: string;
  onChangeText: (typedTime: string) => void;
  onBlur?: () => void;
  errorMessage?: string;
  isRequired?: boolean;
  fieldSurface?: TextFieldSurface;
  minuteStep?: number;
}

const defaultMinuteStep = 5;
const timeSeparator = ":";

function twoDigits(timePart: number): string {
  return String(timePart).padStart(2, "0");
}

function timePartsOf(typedTime: string): [number | null, number | null] {
  if (!startTimePattern.test(typedTime)) {
    return [null, null];
  }
  const [hour, minute] = typedTime.split(timeSeparator).map(Number);
  return [hour ?? null, minute ?? null];
}

export function TimeField({
  label,
  hint,
  value,
  onChangeText,
  onBlur,
  errorMessage,
  isRequired = false,
  fieldSurface,
  minuteStep = defaultMinuteStep,
}: TimeFieldProps) {
  const [pickedHour, setPickedHour] = useState<number | null>(null);
  const [pickedMinute, setPickedMinute] = useState<number | null>(null);
  const [isPickerOpen, setIsPickerOpen] = useState(false);

  const openPicker = () => {
    const [typedHour, typedMinute] = timePartsOf(value);
    setPickedHour(typedHour);
    setPickedMinute(typedMinute);
    setIsPickerOpen(true);
  };

  const confirmPickedTime = () => {
    if (pickedHour !== null && pickedMinute !== null) {
      onChangeText(`${twoDigits(pickedHour)}${timeSeparator}${twoDigits(pickedMinute)}`);
    }
    setIsPickerOpen(false);
    onBlur?.();
  };

  return (
    <>
      <TextField
        label={label}
        hint={hint}
        isRequired={isRequired}
        placeholder={translate("timeField.placeholder")}
        keyboardType="number-pad"
        fieldSurface={fieldSurface}
        value={value}
        onChangeText={(typedText) => onChangeText(formatStartTimeAsTyped(typedText))}
        onBlur={onBlur}
        errorMessage={errorMessage}
        accessory={
          <IconButton
            icon="pickTime"
            tone="muted-foreground"
            accessibilityLabel={translate("timeField.openPicker")}
            accessibilityHint={label}
            onPress={openPicker}
          />
        }
      />
      {isPickerOpen ? (
        <PickerSheet
          onCancel={() => setIsPickerOpen(false)}
          onDone={confirmPickedTime}
          isDoneEnabled={pickedHour !== null && pickedMinute !== null}
        >
          <TimePicker
            selectedHour={pickedHour}
            selectedMinute={pickedMinute}
            onSelectHour={setPickedHour}
            onSelectMinute={setPickedMinute}
            minuteStep={minuteStep}
          />
        </PickerSheet>
      ) : null}
    </>
  );
}

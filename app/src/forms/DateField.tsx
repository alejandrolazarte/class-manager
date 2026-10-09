import { useState } from "react";
import { CalendarPicker } from "@/forms/CalendarPicker";
import { PickerSheet } from "@/forms/PickerSheet";
import {
  formatBirthDateAsTyped,
  formatBirthDateForDisplay,
  parseBirthDate,
} from "@/features/students/birthDateFormatting";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { IconButton } from "@/ui/IconButton";
import { TextField, TextFieldSurface } from "@/ui/TextField";

interface DateFieldProps {
  label: string;
  hint?: string;
  placeholder?: string;
  value: string;
  onChangeText: (typedDate: string) => void;
  onBlur?: () => void;
  errorMessage?: string;
  isRequired?: boolean;
  fieldSurface?: TextFieldSurface;
  earliestIsoDate?: string;
  latestIsoDate?: string;
}

const yearsBeforeToday = 100;
const yearsAfterToday = 5;
const isoYearLength = 4;
const firstDayOfYear = "-01-01";
const lastDayOfYear = "-12-31";

function yearsFromToday(yearCount: number, dayOfYear: string): string {
  return `${Number(todayIsoDate().slice(0, isoYearLength)) + yearCount}${dayOfYear}`;
}

export function DateField({
  label,
  hint,
  placeholder = translate("dateField.placeholder"),
  value,
  onChangeText,
  onBlur,
  errorMessage,
  isRequired = false,
  fieldSurface,
  earliestIsoDate = yearsFromToday(-yearsBeforeToday, firstDayOfYear),
  latestIsoDate = yearsFromToday(yearsAfterToday, lastDayOfYear),
}: DateFieldProps) {
  const [pickedIsoDate, setPickedIsoDate] = useState<string | null>(null);
  const [isPickerOpen, setIsPickerOpen] = useState(false);

  const openPicker = () => {
    setPickedIsoDate(parseBirthDate(value));
    setIsPickerOpen(true);
  };

  const confirmPickedDate = () => {
    if (pickedIsoDate !== null) {
      onChangeText(formatBirthDateForDisplay(pickedIsoDate));
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
        placeholder={placeholder}
        keyboardType="number-pad"
        fieldSurface={fieldSurface}
        value={value}
        onChangeText={(typedText) => onChangeText(formatBirthDateAsTyped(typedText))}
        onBlur={onBlur}
        errorMessage={errorMessage}
        accessory={
          <IconButton
            icon="pickDate"
            tone="muted-foreground"
            accessibilityLabel={translate("dateField.openCalendar")}
            accessibilityHint={label}
            onPress={openPicker}
          />
        }
      />
      {isPickerOpen ? (
        <PickerSheet
          onCancel={() => setIsPickerOpen(false)}
          onDone={confirmPickedDate}
          isDoneEnabled={pickedIsoDate !== null}
        >
          <CalendarPicker
            selectedIsoDate={pickedIsoDate}
            onSelect={setPickedIsoDate}
            earliestIsoDate={earliestIsoDate}
            latestIsoDate={latestIsoDate}
          />
        </PickerSheet>
      ) : null}
    </>
  );
}

import { useState } from "react";
import { View } from "react-native";
import { minutesOf, secondsOf } from "@/forms/duration";
import { DurationPickerSheet } from "@/forms/DurationPickerSheet";
import { twoDigits } from "@/forms/TimeColumn";
import { translate } from "@/i18n/translate";
import { SelectField } from "@/ui/SelectField";

interface DurationFieldsProps {
  minutesLabel: string;
  secondsLabel: string;
  valueInSeconds: number | null;
  onChange: (valueInSeconds: number) => void;
  isRequired?: boolean;
}

export function DurationFields({
  minutesLabel,
  secondsLabel,
  valueInSeconds,
  onChange,
  isRequired = false,
}: DurationFieldsProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const openPicker = () => setIsPickerOpen(true);

  return (
    <>
      <View className="flex-row gap-2">
        <View className="flex-1">
          <SelectField
            label={minutesLabel}
            icon="pickTime"
            value={valueInSeconds === null ? null : twoDigits(minutesOf(valueInSeconds))}
            placeholder={translate("durationField.minutesPlaceholder")}
            accessibilityHint={translate("durationField.openPicker")}
            onPress={openPicker}
          />
        </View>
        <View className="flex-1">
          <SelectField
            label={secondsLabel}
            icon="pickTime"
            isRequired={isRequired}
            value={valueInSeconds === null ? null : twoDigits(secondsOf(valueInSeconds))}
            placeholder={translate("durationField.secondsPlaceholder")}
            accessibilityHint={translate("durationField.openPicker")}
            onPress={openPicker}
          />
        </View>
      </View>
      {isPickerOpen ? (
        <DurationPickerSheet
          initialValueInSeconds={valueInSeconds}
          onCancel={() => setIsPickerOpen(false)}
          onDone={(pickedValueInSeconds) => {
            onChange(pickedValueInSeconds);
            setIsPickerOpen(false);
          }}
        />
      ) : null}
    </>
  );
}

import { useState } from "react";
import { View } from "react-native";
import {
  durationInSeconds,
  minuteOptions,
  minutesOf,
  secondOptions,
  secondsOf,
} from "@/forms/duration";
import { PickerSheet } from "@/forms/PickerSheet";
import { TimeColumn } from "@/forms/TimeColumn";
import { translate } from "@/i18n/translate";

interface DurationPickerSheetProps {
  initialValueInSeconds: number | null;
  onCancel: () => void;
  onDone: (valueInSeconds: number) => void;
}

export function DurationPickerSheet({
  initialValueInSeconds,
  onCancel,
  onDone,
}: DurationPickerSheetProps) {
  const [pickedMinute, setPickedMinute] = useState(
    initialValueInSeconds === null ? 0 : minutesOf(initialValueInSeconds),
  );
  const [pickedSecond, setPickedSecond] = useState<number | null>(
    initialValueInSeconds === null ? null : secondsOf(initialValueInSeconds),
  );

  const confirmPickedDuration = () => {
    if (pickedSecond !== null) {
      onDone(durationInSeconds(pickedMinute, pickedSecond));
    }
  };

  return (
    <PickerSheet
      onCancel={onCancel}
      onDone={confirmPickedDuration}
      isDoneEnabled={pickedSecond !== null}
    >
      <View className="flex-row gap-3">
        <TimeColumn
          title={translate("durationField.minutes")}
          options={minuteOptions}
          selectedOption={pickedMinute}
          initiallyVisibleOption={0}
          onSelect={setPickedMinute}
        />
        <TimeColumn
          title={translate("durationField.seconds")}
          options={secondOptions}
          selectedOption={pickedSecond}
          initiallyVisibleOption={0}
          onSelect={setPickedSecond}
        />
      </View>
    </PickerSheet>
  );
}

import { PropsWithChildren } from "react";
import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";

interface PickerSheetProps extends PropsWithChildren {
  onCancel: () => void;
  onDone: () => void;
  isDoneEnabled: boolean;
}

export function PickerSheet({ onCancel, onDone, isDoneEnabled, children }: PickerSheetProps) {
  return (
    <BottomSheet onClose={onCancel}>
      {children}
      <View className="flex-row gap-2.5">
        <View className="flex-1">
          <Button variant="secondary" label={translate("common.cancel")} onPress={onCancel} />
        </View>
        <View className="flex-1">
          <Button label={translate("dateField.done")} onPress={onDone} disabled={!isDoneEnabled} />
        </View>
      </View>
    </BottomSheet>
  );
}

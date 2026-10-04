import { View } from "react-native";
import { DeliveryClass } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";

interface DeliveryPickerProps {
  deliveryClasses: DeliveryClass[];
  classGroupId: string | null;
  onChange: (classGroupId: string | null) => void;
}

export function DeliveryPicker({ deliveryClasses, classGroupId, onChange }: DeliveryPickerProps) {
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {translate("delivery.title")}
      </AppText>
      <View className="flex-row flex-wrap gap-2">
        <Chip
          label={translate("delivery.pickup")}
          isSelected={classGroupId === null}
          onPress={() => onChange(null)}
        />
        {deliveryClasses.map((deliveryClass) => (
          <Chip
            key={`${deliveryClass.classGroupId}-${deliveryClass.studentFullName}`}
            label={translate("delivery.inClass", {
              className: deliveryClass.classGroupName,
              student: deliveryClass.studentFullName,
            })}
            isSelected={classGroupId === deliveryClass.classGroupId}
            onPress={() => onChange(deliveryClass.classGroupId)}
          />
        ))}
      </View>
    </View>
  );
}

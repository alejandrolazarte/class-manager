import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface QuantityStepperProps {
  units: number;
  minimum: number;
  maximum: number;
  onChange: (units: number) => void;
  decreaseLabel?: string;
  increaseLabel?: string;
}

export function QuantityStepper({
  units,
  minimum,
  maximum,
  onChange,
  decreaseLabel = translate("student.shop.decrease"),
  increaseLabel = translate("student.shop.increase"),
}: QuantityStepperProps) {
  return (
    <View className="flex-row items-center">
      <IconButton
        icon="remove"
        tone="primary"
        accessibilityLabel={decreaseLabel}
        disabled={units <= minimum}
        onPress={() => onChange(units - 1)}
      />
      <AppText variant="heading" className="min-w-6 text-center font-heavy">
        {units}
      </AppText>
      <IconButton
        icon="add"
        tone="primary"
        accessibilityLabel={increaseLabel}
        disabled={units >= maximum}
        onPress={() => onChange(units + 1)}
      />
    </View>
  );
}

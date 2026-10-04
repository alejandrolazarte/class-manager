import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconName } from "@/ui/Icon";
import { IconButton } from "@/ui/IconButton";

interface QuantityStepperProps {
  units: number;
  minimum: number;
  maximum: number;
  onChange: (units: number) => void;
  decreaseLabel?: string;
  increaseLabel?: string;
  decreaseIcon?: IconName;
}

export function QuantityStepper({
  units,
  minimum,
  maximum,
  onChange,
  decreaseLabel = translate("family.shop.decrease"),
  increaseLabel = translate("family.shop.increase"),
  decreaseIcon = "remove",
}: QuantityStepperProps) {
  return (
    <View className="flex-row items-center">
      <IconButton
        icon={decreaseIcon}
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

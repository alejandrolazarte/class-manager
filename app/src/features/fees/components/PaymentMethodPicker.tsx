import { View } from "react-native";
import { paymentMethods } from "@/features/fees/paymentSchema";
import { PaymentMethod } from "@/features/fees/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { IconName } from "@/ui/Icon";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface PaymentMethodPickerProps {
  value: PaymentMethod;
  onChange: (method: PaymentMethod) => void;
  label?: string;
}

const methodIcons: Record<PaymentMethod, IconName> = {
  Cash: "cash",
  Transfer: "transfer",
  Card: "card",
  Other: "otherMethod",
};

export function PaymentMethodPicker({ value, onChange, label }: PaymentMethodPickerProps) {
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label ?? translate("fees.payment.method"))}
      </AppText>
      <View className="flex-row flex-wrap gap-2">
        {paymentMethods.map((method) => (
          <View key={method} className="min-w-[45%] flex-1">
            <Chip
              shape="tile"
              icon={methodIcons[method]}
              label={translate(`fees.methods.${method}` as TranslationKey)}
              isSelected={value === method}
              onPress={() => onChange(method)}
            />
          </View>
        ))}
      </View>
    </View>
  );
}

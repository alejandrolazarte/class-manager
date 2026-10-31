import { View } from "react-native";
import { BusinessBillingMode } from "@/features/fees/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { OptionCard } from "@/ui/OptionCard";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

const billingModes: readonly BusinessBillingMode[] = ["MonthlyFee", "ClassPacks"];

interface BillingModeChoiceProps {
  billingMode: BusinessBillingMode | null;
  onChange: (billingMode: BusinessBillingMode) => void;
  note: string;
}

export function BillingModeChoice({ billingMode, onChange, note }: BillingModeChoiceProps) {
  return (
    <View className="gap-2" accessibilityRole="radiogroup">
      <AppText variant="label" tone="muted">
        {withRequiredMark(translate("fees.billingMode.question"))}
      </AppText>
      {billingModes.map((mode) => (
        <OptionCard
          key={mode}
          label={translate(`fees.billingMode.${mode}`)}
          hint={translate(`fees.billingMode.hint.${mode}`)}
          isSelected={billingMode === mode}
          onPress={() => onChange(mode)}
        />
      ))}
      <AppText variant="caption" tone="muted">
        {note}
      </AppText>
    </View>
  );
}

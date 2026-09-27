import { View } from "react-native";
import { addMonths, formatMonth } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { StepArrow } from "@/ui/StepArrow";

interface EffectiveMonthPickerProps {
  month: string;
  onChange: (month: string) => void;
}

export function EffectiveMonthPicker({ month, onChange }: EffectiveMonthPickerProps) {
  return (
    <View className="gap-1">
      <AppText variant="label" tone="muted">
        {translate("fees.effectiveMonth.label")}
      </AppText>
      <View className="flex-row items-center justify-between rounded-xl border border-border-strong bg-surface">
        <StepArrow
          direction="previous"
          label={translate("fees.effectiveMonth.previous")}
          onPress={() => onChange(addMonths(month, -1))}
        />
        <AppText variant="bodyStrong">{formatMonth(month)}</AppText>
        <StepArrow
          direction="next"
          label={translate("fees.effectiveMonth.next")}
          onPress={() => onChange(addMonths(month, 1))}
        />
      </View>
    </View>
  );
}

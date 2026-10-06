import { View } from "react-native";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface EffectiveMonthPickerProps {
  month: string;
  onChange: (month: string) => void;
  label?: string;
}

export function EffectiveMonthPicker({ month, onChange, label }: EffectiveMonthPickerProps) {
  const isCurrentMonthOrEarlier = month <= monthOf();
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {label ?? translate("fees.effectiveMonth.label")}
      </AppText>
      <View className="h-[52px] flex-row items-center justify-between rounded-2xl border-[1.5px] border-border bg-surface px-1">
        <IconButton
          icon="previous"
          tone="primary"
          accessibilityLabel={translate("fees.effectiveMonth.previous")}
          disabled={isCurrentMonthOrEarlier}
          onPress={() => onChange(addMonths(month, -1))}
        />
        <AppText variant="bodyStrong">{formatMonth(month)}</AppText>
        <IconButton
          icon="next"
          tone="primary"
          accessibilityLabel={translate("fees.effectiveMonth.next")}
          onPress={() => onChange(addMonths(month, 1))}
        />
      </View>
    </View>
  );
}

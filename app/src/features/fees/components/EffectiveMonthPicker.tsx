import { View } from "react-native";
import { addMonths, formatMonth, monthAndYear, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { IconButton } from "@/ui/IconButton";

interface EffectiveMonthPickerProps {
  month: string;
  onChange: (month: string) => void;
  label?: string;
}

interface PastMonthsWarningProps {
  fromMonth: string;
  lastPastMonth: string;
}

function PastMonthsWarning({ fromMonth, lastPastMonth }: PastMonthsWarningProps) {
  const changedMonths =
    fromMonth === lastPastMonth
      ? translate("fees.effectiveMonth.pastWarning.one", { month: formatMonth(fromMonth) })
      : translate("fees.effectiveMonth.pastWarning.other", {
          from: monthAndYear(fromMonth),
          to: monthAndYear(lastPastMonth),
        });
  return (
    <Banner tone="warning" message={translate("fees.effectiveMonth.pastWarning.title")}>
      <AppText>{changedMonths}</AppText>
    </Banner>
  );
}

export function EffectiveMonthPicker({ month, onChange, label }: EffectiveMonthPickerProps) {
  const currentMonth = monthOf();
  const isInThePast = month < currentMonth;
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
      {isInThePast ? (
        <PastMonthsWarning fromMonth={month} lastPastMonth={addMonths(currentMonth, -1)} />
      ) : null}
    </View>
  );
}

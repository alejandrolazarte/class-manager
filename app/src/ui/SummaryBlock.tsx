import { PropsWithChildren } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

interface SummaryBlockProps extends PropsWithChildren {
  label: string;
  amount: string;
  total: string;
  ratio: number;
  accessibilityLabel: string;
}

const completeRatio = 1;

export function SummaryBlock({
  label,
  amount,
  total,
  ratio,
  accessibilityLabel,
  children,
}: SummaryBlockProps) {
  return (
    <View
      accessible
      accessibilityLabel={accessibilityLabel}
      className="gap-2.5 rounded-2xl border border-border px-4 py-3.5"
    >
      <View>
        <AppText variant="caption" tone="subtle">
          {label}
        </AppText>
        <View className="flex-row flex-wrap items-baseline gap-x-2">
          <AppText variant="amount">{amount}</AppText>
          <AppText variant="body" tone="subtle">
            {total}
          </AppText>
        </View>
      </View>
      <View className="flex-row">
        <ProgressBar ratio={ratio} tone={ratio >= completeRatio ? "success" : "primary"} />
      </View>
      {children ? <View className="gap-0.5">{children}</View> : null}
    </View>
  );
}

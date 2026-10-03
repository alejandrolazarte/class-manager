import { ReactNode } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

interface CollectedSummaryCardProps {
  label: string;
  collected: string;
  total: string;
  ratio: number;
  accessibilityLabel: string;
  children: ReactNode;
}

export function CollectedSummaryCard({
  label,
  collected,
  total,
  ratio,
  accessibilityLabel,
  children,
}: CollectedSummaryCardProps) {
  return (
    <View
      accessible
      accessibilityLabel={accessibilityLabel}
      className="gap-3 rounded-3xl bg-primary px-[18px] pb-4 pt-[18px]"
    >
      <View className="gap-0.5">
        <View className="flex-row items-baseline justify-between gap-2">
          <AppText variant="label" tone="onPrimary" className="opacity-85">
            {label}
          </AppText>
          <AppText variant="label" tone="onPrimary" className="opacity-85">
            {total}
          </AppText>
        </View>
        <AppText variant="amount" tone="onPrimary">
          {collected}
        </AppText>
      </View>
      <View className="flex-row">
        <ProgressBar tone="onPrimary" ratio={ratio} />
      </View>
      {children}
    </View>
  );
}

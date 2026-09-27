import { View } from "react-native";
import { MonthlyFees } from "@/features/fees/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

interface MonthlySummaryCardProps {
  monthlyFees: MonthlyFees;
  debtorCount: number;
  owedAmount: number;
  money: (amount: number) => string;
}

export function MonthlySummaryCard({
  monthlyFees,
  debtorCount,
  owedAmount,
  money,
}: MonthlySummaryCardProps) {
  return (
    <View
      accessible
      accessibilityLabel={translate("fees.month.summary", {
        paid: money(monthlyFees.totalPaid),
        due: money(monthlyFees.totalDue),
      })}
      className="gap-3 rounded-3xl bg-primary px-[18px] pb-4 pt-[18px]"
    >
      <View className="gap-0.5">
        <AppText variant="label" tone="onPrimary" className="opacity-85">
          {translate("fees.month.collected")}
        </AppText>
        <View className="flex-row flex-wrap items-baseline gap-2">
          <AppText variant="amount" tone="onPrimary">
            {money(monthlyFees.totalPaid)}
          </AppText>
          <AppText variant="bodyStrong" tone="onPrimary" className="font-label opacity-85">
            {translate("fees.month.ofDue", { due: money(monthlyFees.totalDue) })}
          </AppText>
        </View>
      </View>
      <View className="flex-row">
        <ProgressBar
          tone="onPrimary"
          ratio={monthlyFees.totalDue > 0 ? monthlyFees.totalPaid / monthlyFees.totalDue : 0}
        />
      </View>
      <AppText variant="label" tone="onPrimary" className="opacity-90">
        {debtorCount > 0
          ? translateCount("fees.month.owing", debtorCount, { amount: money(owedAmount) })
          : translate("fees.month.everybodyPaid")}
      </AppText>
    </View>
  );
}

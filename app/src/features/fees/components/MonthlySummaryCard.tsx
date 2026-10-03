import { MonthlyFees } from "@/features/fees/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { CollectedSummaryCard } from "@/ui/CollectedSummaryCard";

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
    <CollectedSummaryCard
      label={translate("fees.month.collected")}
      collected={money(monthlyFees.totalPaid)}
      total={translate("fees.month.total", { due: money(monthlyFees.totalDue) })}
      ratio={monthlyFees.totalDue > 0 ? monthlyFees.totalPaid / monthlyFees.totalDue : 0}
      accessibilityLabel={translate("fees.month.summary", {
        paid: money(monthlyFees.totalPaid),
        due: money(monthlyFees.totalDue),
      })}
    >
      <AppText variant="label" tone="onPrimary" className="opacity-90">
        {debtorCount > 0
          ? translateCount("fees.month.owing", debtorCount, { amount: money(owedAmount) })
          : translate("fees.month.everybodyPaid")}
      </AppText>
    </CollectedSummaryCard>
  );
}

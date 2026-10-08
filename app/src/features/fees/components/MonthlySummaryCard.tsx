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
  const paidByMonthlyClients = monthlyFees.clients.reduce(
    (total, client) => total + client.paid,
    0,
  );
  const paidByClassPackClients = monthlyFees.totalPaid - paidByMonthlyClients;
  const statusMessage =
    debtorCount > 0
      ? translateCount("fees.month.owing", debtorCount, { amount: money(owedAmount) })
      : translate(monthlyFees.totalDue > 0 ? "fees.month.everybodyPaid" : "fees.month.noFeeDue");
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
        {statusMessage}
      </AppText>
      {paidByClassPackClients > 0 ? (
        <AppText variant="caption" tone="onPrimary" className="opacity-90">
          {translate("fees.month.paidByClassPackClients", {
            amount: money(paidByClassPackClients),
          })}
        </AppText>
      ) : null}
    </CollectedSummaryCard>
  );
}

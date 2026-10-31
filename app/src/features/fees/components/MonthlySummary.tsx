import { MonthlyFees } from "@/features/fees/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { SummaryBlock } from "@/ui/SummaryBlock";

interface MonthlySummaryProps {
  monthlyFees: MonthlyFees;
  debtorCount: number;
  classDebtorCount: number;
  owedAmount: number;
  money: (amount: number) => string;
}

export function MonthlySummary({
  monthlyFees,
  debtorCount,
  classDebtorCount,
  owedAmount,
  money,
}: MonthlySummaryProps) {
  const paidByMonthlyClients = monthlyFees.clients.reduce(
    (total, client) => total + client.paid,
    0,
  );
  const paidByClassPackClients = monthlyFees.totalPaid - paidByMonthlyClients;
  const hasFeeDue = monthlyFees.totalDue > 0;
  const statusMessage =
    debtorCount > 0
      ? translateCount("fees.month.owing", debtorCount, { amount: money(owedAmount) })
      : translate(hasFeeDue ? "fees.month.everybodyPaid" : "fees.month.noFeeDue");
  const statusTone: TextTone = debtorCount > 0 ? "warning" : hasFeeDue ? "success" : "muted";
  return (
    <SummaryBlock
      label={translate("fees.month.collected")}
      amount={money(monthlyFees.totalPaid)}
      total={translate("fees.month.total", { due: money(monthlyFees.totalDue) })}
      ratio={hasFeeDue ? monthlyFees.totalPaid / monthlyFees.totalDue : 0}
      accessibilityLabel={translate("fees.month.summary", {
        paid: money(monthlyFees.totalPaid),
        due: money(monthlyFees.totalDue),
      })}
    >
      <AppText variant="label" tone={statusTone}>
        {statusMessage}
      </AppText>
      {classDebtorCount > 0 ? (
        <AppText variant="label" tone="warning">
          {translateCount("fees.month.owingClasses", classDebtorCount)}
        </AppText>
      ) : null}
      {monthlyFees.classPackSales > 0 ? (
        <AppText variant="footnote" tone="subtle">
          {translate("fees.month.classPackSales", { amount: money(monthlyFees.classPackSales) })}
        </AppText>
      ) : null}
      {paidByClassPackClients > 0 ? (
        <AppText variant="footnote" tone="subtle">
          {translate("fees.month.paidByClassPackClients", {
            amount: money(paidByClassPackClients),
          })}
        </AppText>
      ) : null}
    </SummaryBlock>
  );
}

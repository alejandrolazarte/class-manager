import { View } from "react-native";
import { monthName } from "@/features/fees/months";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { SummaryBlock } from "@/ui/SummaryBlock";

export interface CollectionsTotals {
  month: string;
  plansCollected: number;
  classPacksCollected: number;
  productsCollected: number;
  owed: number;
  debtorCount: number;
  paidByClassPackClients: number;
}

interface CollectionsSummaryProps {
  totals: CollectionsTotals;
  awaitingPickupCount: number | null;
  money: (amount: number) => string;
}

export function CollectionsSummary({
  totals,
  awaitingPickupCount,
  money,
}: CollectionsSummaryProps) {
  const classesCollected = totals.plansCollected + totals.classPacksCollected;
  const collected = classesCollected + totals.productsCollected;
  const total = collected + totals.owed;
  const showsOrders = awaitingPickupCount !== null;
  return (
    <SummaryBlock
      label={translate("collections.collectedIn", { month: monthName(totals.month) })}
      amount={money(collected)}
      total={translate("collections.total", { total: money(total) })}
      ratio={total > 0 ? collected / total : 0}
      accessibilityLabel={translate("collections.summary", {
        month: monthName(totals.month),
        collected: money(collected),
        total: money(total),
      })}
    >
      {showsOrders ? (
        <View className="flex-row gap-2">
          <OutlinedBlock className="min-w-0 flex-1 gap-0.5 px-3 py-2.5">
            <AppText variant="caption" tone="subtle">
              {translate("collections.classes")}
            </AppText>
            <AppText variant="headline">{money(classesCollected)}</AppText>
            <AppText variant="footnote" tone="subtle">
              {translate("collections.classesDetail", {
                plans: money(totals.plansCollected),
                classPacks: money(totals.classPacksCollected),
              })}
            </AppText>
          </OutlinedBlock>
          <OutlinedBlock className="min-w-0 flex-1 gap-0.5 px-3 py-2.5">
            <AppText variant="caption" tone="subtle">
              {translate("collections.products")}
            </AppText>
            <AppText variant="headline">{money(totals.productsCollected)}</AppText>
          </OutlinedBlock>
        </View>
      ) : null}
      <View className="flex-row flex-wrap justify-between gap-x-2 gap-y-0.5">
        <AppText
          variant="label"
          tone={totals.debtorCount > 0 ? "warning" : total > 0 ? "success" : "muted"}
        >
          {totals.debtorCount > 0
            ? translateCount("collections.owing", totals.debtorCount, {
                amount: money(totals.owed),
              })
            : translate(total > 0 ? "collections.everythingCollected" : "collections.empty")}
        </AppText>
        {showsOrders ? (
          <AppText variant="label" tone="muted">
            {translate("collections.toHandOverCount", { count: awaitingPickupCount })}
          </AppText>
        ) : null}
      </View>
      {totals.paidByClassPackClients > 0 ? (
        <AppText variant="footnote" tone="subtle">
          {translate("fees.month.paidByClassPackClients", {
            amount: money(totals.paidByClassPackClients),
          })}
        </AppText>
      ) : null}
    </SummaryBlock>
  );
}

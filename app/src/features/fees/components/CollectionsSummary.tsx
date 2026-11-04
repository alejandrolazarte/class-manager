import { View } from "react-native";
import { monthName } from "@/features/fees/months";
import { translate, translateCount } from "@/i18n/translate";
import { AmountBreakdown } from "@/ui/AmountBreakdown";
import { AppText } from "@/ui/AppText";
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
        <AmountBreakdown
          items={[
            {
              key: "classes",
              label: translate("collections.classes"),
              amount: money(classesCollected),
            },
            {
              key: "plans",
              label: translate("collections.kind.plans"),
              amount: money(totals.plansCollected),
              isNested: true,
            },
            {
              key: "classPacks",
              label: translate("collections.kind.classPacks"),
              amount: money(totals.classPacksCollected),
              isNested: true,
            },
            {
              key: "products",
              label: translate("collections.products"),
              amount: money(totals.productsCollected),
            },
          ]}
        />
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

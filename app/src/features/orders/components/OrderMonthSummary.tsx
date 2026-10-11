import { View } from "react-native";
import { monthName } from "@/features/fees/months";
import { OrderSummary } from "@/features/orders/types";
import { PendingOrders } from "@/features/orders/useOrders";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { SummaryBlock } from "@/ui/SummaryBlock";

interface OrderMonthSummaryProps {
  orderSummary: OrderSummary;
  pendingOrders: PendingOrders;
  money: (amount: number) => string;
}

export function OrderMonthSummary({ orderSummary, pendingOrders, money }: OrderMonthSummaryProps) {
  const total = orderSummary.collected + orderSummary.unpaid;
  const unpaidCount = pendingOrders.requested.length;
  return (
    <SummaryBlock
      label={translate("collections.collectedIn", { month: monthName(orderSummary.month) })}
      amount={money(orderSummary.collected)}
      total={translate("collections.total", { total: money(total) })}
      ratio={total > 0 ? orderSummary.collected / total : 0}
      accessibilityLabel={translate("collections.summary", {
        collected: money(orderSummary.collected),
        total: money(total),
      })}
    >
      <View className="flex-row flex-wrap justify-between gap-x-2 gap-y-0.5">
        <AppText variant="label" tone={unpaidCount > 0 ? "warning" : "success"}>
          {unpaidCount > 0
            ? translateCount("collections.unpaidOrders", unpaidCount, {
                amount: money(orderSummary.unpaid),
              })
            : translate("collections.allOrdersPaid")}
        </AppText>
        <AppText variant="label" tone="muted">
          {translate("collections.toHandOverCount", { count: pendingOrders.awaitingPickup.length })}
        </AppText>
      </View>
      {orderSummary.classPackSales > 0 ? (
        <AppText variant="footnote" tone="subtle">
          {translate("collections.classPackSales", { amount: money(orderSummary.classPackSales) })}
        </AppText>
      ) : null}
    </SummaryBlock>
  );
}

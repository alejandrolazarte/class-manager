import { View } from "react-native";
import { PendingOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";

interface PendingOrdersSummaryCardProps {
  pendingOrders: PendingOrders;
  money: (amount: number) => string;
}

export function PendingOrdersSummaryCard({ pendingOrders, money }: PendingOrdersSummaryCardProps) {
  const unpaidTotal = pendingOrders.requested.reduce((total, order) => total + order.total, 0);
  return (
    <Card className="gap-2.5 p-4">
      <View className="flex-row items-baseline justify-between gap-2">
        <AppText variant="overline" tone="subtle">
          {translate("collections.unpaidOrders")}
        </AppText>
        <AppText variant="label" tone="subtle">
          {translate("collections.inProgress", { count: pendingOrders.count })}
        </AppText>
      </View>
      <AppText variant="amount">{money(unpaidTotal)}</AppText>
      <View className="flex-row justify-between gap-2">
        <AppText variant="label" tone={pendingOrders.requested.length > 0 ? "danger" : "subtle"}>
          {translate("collections.unpaidCount", { count: pendingOrders.requested.length })}
        </AppText>
        <AppText variant="label" tone="subtle">
          {translate("collections.toHandOverCount", { count: pendingOrders.awaitingPickup.length })}
        </AppText>
      </View>
    </Card>
  );
}

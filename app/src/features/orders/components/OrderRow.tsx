import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { CollectSheet } from "@/features/orders/components/CollectSheet";
import {
  orderClientLabel,
  orderDeliveryLabel,
  orderMoneyLabel,
  orderStatusLabel,
  orderStatusTone,
  orderSummaryLine,
} from "@/features/orders/orderLabels";
import { Order } from "@/features/orders/types";
import { useOrderActions } from "@/features/orders/useOrderActions";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { orderTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill } from "@/ui/StatusPill";

interface OrderRowProps {
  order: Order;
  canManage: boolean;
}

export function OrderRow({ order, canManage }: OrderRowProps) {
  const router = useRouter();
  const orderTab = useCurrentTab(orderTabs);
  const { markDelivered, isDelivering } = useOrderActions(order);
  const [isCollecting, setIsCollecting] = useState(false);
  const isOwing = order.status !== "Cancelled" && order.balance > 0;
  const isPartlyPaid = isOwing && order.paidAmount > 0;
  const moneyLabel = orderMoneyLabel(order);
  const canCollect = canManage && isOwing;
  const canDeliver = canManage && !canCollect && order.status === "Pending";

  return (
    <View className="border-b border-border-subtle pb-1">
      <HairlineRow
        hasDivider={false}
        className="gap-1 py-3"
        accessibilityLabel={translate("orders.open", {
          number: order.number,
          client: orderClientLabel(order),
        })}
        onPress={() => router.push(routes.order(orderTab, order.id))}
      >
        <View className="flex-row items-start justify-between gap-3">
          <AppText variant="bodyStrong" className="min-w-0 flex-1">
            {orderClientLabel(order)}
          </AppText>
          <AppText variant="bodyStrong">{formatMoney(order.total, order.currencyCode)}</AppText>
        </View>
        <AppText variant="footnote" tone="subtle">
          {orderSummaryLine(order)}
        </AppText>
        {moneyLabel === null ? null : (
          <AppText variant="footnote" tone={moneyLabel.tone}>
            {moneyLabel.text}
          </AppText>
        )}
        {order.awaitsPickup ? (
          <AppText variant="footnote" tone="subtle">
            {orderDeliveryLabel(order)}
          </AppText>
        ) : null}
        {isPartlyPaid ? (
          <ProgressBar ratio={order.paidAmount / (order.paidAmount + order.balance)} isThin />
        ) : null}
        <View className="mt-1 flex-row flex-wrap items-center gap-1.5">
          <StatusPill label={orderStatusLabel(order)} tone={orderStatusTone(order)} isSmall />
          {order.awaitsPickup && order.readyAt !== null ? (
            <StatusPill label={translate("orders.readyPill")} tone="primary" isSmall />
          ) : null}
        </View>
      </HairlineRow>
      {canCollect || canDeliver ? (
        <View className="flex-row justify-end pb-2">
          {canCollect ? (
            <Button
              size="small"
              icon="paid"
              label={translate("orders.collect")}
              onPress={() => setIsCollecting(true)}
            />
          ) : (
            <Button
              size="small"
              label={translate("orders.markDelivered")}
              onPress={markDelivered}
              isLoading={isDelivering}
            />
          )}
        </View>
      ) : null}
      {isCollecting ? <CollectSheet order={order} onClose={() => setIsCollecting(false)} /> : null}
    </View>
  );
}

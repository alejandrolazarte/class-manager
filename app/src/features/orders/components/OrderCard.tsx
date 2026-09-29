import { useState } from "react";
import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { RefundPanel } from "@/features/orders/components/RefundPanel";
import { orderClientLabel, orderStatusLabel } from "@/features/orders/orderLabels";
import { Order } from "@/features/orders/types";
import { useMarkOrderDelivered } from "@/features/orders/useOrderMutations";
import { formatLongDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { useToast } from "@/ui/ToastProvider";

interface OrderCardProps {
  order: Order;
  canManage: boolean;
}

export function OrderCard({ order, canManage }: OrderCardProps) {
  const currencyCode = useBusinessCurrency();
  const { showToast } = useToast();
  const markOrderDeliveredMutation = useMarkOrderDelivered();
  const [isRefunding, setIsRefunding] = useState(false);

  const markDelivered = async () => {
    try {
      await markOrderDeliveredMutation.mutateAsync(order.id);
      showToast(translate("orders.delivered"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <Card className="gap-2 p-4" accessibilityLabel={orderClientLabel(order)}>
      <View className="flex-row items-start justify-between gap-3">
        <View className="flex-1 gap-0.5">
          <AppText variant="bodyStrong">{orderClientLabel(order)}</AppText>
          <AppText variant="caption" tone="subtle">
            {[order.paidOn ? formatLongDate(order.paidOn) : null, orderStatusLabel(order)]
              .filter(Boolean)
              .join(" · ")}
          </AppText>
        </View>
        <AppText variant="bodyStrong">{formatMoney(order.total, currencyCode)}</AppText>
      </View>
      {order.lines.map((line) => (
        <AppText key={line.id} variant="body" tone="muted">
          {[
            line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name,
            formatMoney(line.total, currencyCode),
            line.refundedAmount > 0
              ? translate("orders.refund.refunded", {
                  amount: formatMoney(line.refundedAmount, currencyCode),
                })
              : null,
          ]
            .filter(Boolean)
            .join(" · ")}
        </AppText>
      ))}
      {canManage ? (
        <View className="flex-row flex-wrap gap-2">
          {order.awaitsPickup ? (
            <Button
              size="medium"
              icon="delivered"
              label={translate("orders.markDelivered")}
              onPress={markDelivered}
              isLoading={markOrderDeliveredMutation.isPending}
            />
          ) : null}
          <Button
            size="medium"
            variant="ghost"
            icon="refund"
            label={translate(isRefunding ? "common.close" : "orders.refund.open")}
            onPress={() => setIsRefunding(!isRefunding)}
          />
        </View>
      ) : null}
      {isRefunding ? <RefundPanel order={order} onDone={() => setIsRefunding(false)} /> : null}
    </Card>
  );
}

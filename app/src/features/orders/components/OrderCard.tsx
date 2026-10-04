import { useState } from "react";
import { Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { PaymentPanel } from "@/features/orders/components/PaymentPanel";
import { RefundPanel } from "@/features/orders/components/RefundPanel";
import { refundableLinesOf } from "@/features/orders/refunds";
import { orderClientLabel, orderStatusLabel } from "@/features/orders/orderLabels";
import { Order } from "@/features/orders/types";
import { useMarkOrderDelivered, useMarkOrderReady } from "@/features/orders/useOrderMutations";
import { formatLongDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Spinner } from "@/ui/Spinner";
import { StatusPill, StatusTone } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

type OrderPanel = "payment" | "refund";

interface OrderCardProps {
  order: Order;
  canManage: boolean;
}

function orderStatusTone(order: Order): StatusTone {
  if (order.awaitsPickup) {
    return "warning";
  }
  if (order.status === "Requested") {
    return "danger";
  }
  if (order.status === "Cancelled" || order.refundedAmount >= order.total) {
    return "neutral";
  }
  return "success";
}

interface OrderActionProps {
  label: string;
  onPress: () => void;
  isLoading?: boolean;
  isExpanded?: boolean;
}

function OrderAction({ label, onPress, isLoading = false, isExpanded }: OrderActionProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      accessibilityState={{ busy: isLoading, expanded: isExpanded }}
      disabled={isLoading}
      onPress={onPress}
      className="min-h-10 flex-row items-center gap-1.5 rounded-full px-2 active:bg-muted"
    >
      {isLoading ? <Spinner tone="primary" /> : null}
      <AppText variant="bodyStrong" tone="primary">
        {label}
      </AppText>
    </Pressable>
  );
}

export function OrderCard({ order, canManage }: OrderCardProps) {
  const currencyCode = useBusinessCurrency();
  const { showToast } = useToast();
  const markOrderDeliveredMutation = useMarkOrderDelivered();
  const markOrderReadyMutation = useMarkOrderReady();
  const [openPanel, setOpenPanel] = useState<OrderPanel | null>(null);
  const hasRefundableLines = refundableLinesOf(order).length > 0;
  const isSettled = order.status === "Paid" || order.status === "Delivered";
  const money = (amount: number) => formatMoney(amount, currencyCode);

  const markDelivered = async () => {
    try {
      await markOrderDeliveredMutation.mutateAsync(order.id);
      showToast(translate("orders.delivered"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  const markReady = async () => {
    try {
      await markOrderReadyMutation.mutateAsync(order.id);
      showToast(translate("orders.readyNotified"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  const togglePanel = (panel: OrderPanel) => setOpenPanel(openPanel === panel ? null : panel);

  const hasProducts = order.lines.some((line) => line.kind === "Product");
  const deliveryLabel =
    order.delivery === "InClass" && order.deliveryClassGroupName !== null
      ? translate("orders.deliveryInClass", { className: order.deliveryClassGroupName })
      : translate("orders.deliveryPickup");
  const lineSummary = order.lines
    .map((line) => (line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name))
    .join(", ");

  return (
    <Card className="gap-2 p-4" accessibilityLabel={orderClientLabel(order)}>
      <View className="flex-row items-start justify-between gap-3">
        <AppText variant="bodyStrong" className="min-w-0 flex-1">
          {orderClientLabel(order)}
        </AppText>
        <AppText variant="bodyStrong" className="font-heavy">
          {money(order.total)}
        </AppText>
      </View>
      <AppText variant="caption" tone="subtle">
        {[
          translate("orders.number", { number: order.number }),
          formatLongDate(order.paidOn ?? order.createdAt.slice(0, isoDateLength)),
          order.channel === "App" ? translate("orders.fromApp") : null,
          lineSummary,
        ]
          .filter(Boolean)
          .join(" · ")}
      </AppText>
      {order.refundedAmount > 0 ? (
        <AppText variant="caption" tone="subtle">
          {translate("orders.refundedTotal", { amount: money(order.refundedAmount) })}
        </AppText>
      ) : null}
      {hasProducts && (order.awaitsPickup || order.status === "Requested") ? (
        <AppText variant="caption" tone="subtle">
          {deliveryLabel}
        </AppText>
      ) : null}
      <View className="flex-row flex-wrap items-center gap-x-1 gap-y-1">
        <View className="flex-1 flex-row flex-wrap items-center gap-1.5">
          <StatusPill label={orderStatusLabel(order)} tone={orderStatusTone(order)} isSmall />
          {order.awaitsPickup && order.readyAt !== null ? (
            <StatusPill label={translate("orders.readyPill")} tone="primary" isSmall />
          ) : null}
        </View>
        {canManage && order.status === "Requested" ? (
          <OrderAction
            label={translate("orders.collect")}
            isExpanded={openPanel === "payment"}
            onPress={() => togglePanel("payment")}
          />
        ) : null}
        {canManage && isSettled && order.awaitsPickup && order.readyAt === null ? (
          <OrderAction
            label={translate("orders.markReady")}
            onPress={markReady}
            isLoading={markOrderReadyMutation.isPending}
          />
        ) : null}
        {canManage && isSettled && order.awaitsPickup ? (
          <OrderAction
            label={translate("orders.markDelivered")}
            onPress={markDelivered}
            isLoading={markOrderDeliveredMutation.isPending}
          />
        ) : null}
        {canManage && isSettled && hasRefundableLines ? (
          <OrderAction
            label={translate(openPanel === "refund" ? "common.close" : "orders.refund.open")}
            isExpanded={openPanel === "refund"}
            onPress={() => togglePanel("refund")}
          />
        ) : null}
      </View>
      {canManage && order.status === "Requested" && openPanel === "payment" ? (
        <PaymentPanel order={order} />
      ) : null}
      {canManage && isSettled && hasRefundableLines && openPanel === "refund" ? (
        <RefundPanel order={order} onDone={() => setOpenPanel(null)} />
      ) : null}
    </Card>
  );
}

import { useState } from "react";
import { View } from "react-native";
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
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { TonalBlock } from "@/ui/TonalBlock";
import { StatusPill, StatusTone } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { Collapsible } from "@/ui/motion/Collapsible";

const isoDateLength = 10;

type OrderPanel = "payment" | "refund";

interface OrderRowProps {
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
    <Button
      variant="ghost"
      size="small"
      label={label}
      onPress={onPress}
      isLoading={isLoading}
      isExpanded={isExpanded}
    />
  );
}

export function OrderRow({ order, canManage }: OrderRowProps) {
  const { showToast } = useToast();
  const markOrderDeliveredMutation = useMarkOrderDelivered();
  const markOrderReadyMutation = useMarkOrderReady();
  const [openPanel, setOpenPanel] = useState<OrderPanel | null>(null);
  const hasRefundableLines = refundableLinesOf(order).length > 0;
  const isSettled = order.status === "Paid" || order.status === "Delivered";
  const money = (amount: number) => formatMoney(amount, order.currencyCode);

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
    <HairlineRow className="gap-1 py-3">
      <View className="flex-row items-start justify-between gap-3">
        <AppText variant="bodyStrong" className="min-w-0 flex-1">
          {orderClientLabel(order)}
        </AppText>
        <AppText variant="bodyStrong">{money(order.total)}</AppText>
      </View>
      <AppText variant="footnote" tone="subtle">
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
        <AppText variant="footnote" tone="subtle">
          {translate("orders.refundedTotal", { amount: money(order.refundedAmount) })}
        </AppText>
      ) : null}
      {hasProducts && (order.awaitsPickup || order.status === "Requested") ? (
        <AppText variant="footnote" tone="subtle">
          {deliveryLabel}
        </AppText>
      ) : null}
      <View className="mt-1 flex-row flex-wrap items-center justify-between gap-x-2 gap-y-1">
        <View className="flex-row flex-wrap items-center gap-1.5">
          <StatusPill label={orderStatusLabel(order)} tone={orderStatusTone(order)} isSmall />
          {order.awaitsPickup && order.readyAt !== null ? (
            <StatusPill label={translate("orders.readyPill")} tone="primary" isSmall />
          ) : null}
        </View>
        <View className="ml-auto flex-row flex-wrap items-center justify-end">
          {canManage && order.status === "Requested" ? (
            <OrderAction
              label={translate(openPanel === "payment" ? "common.close" : "orders.collect")}
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
      </View>
      {canManage && order.status === "Requested" ? (
        <Collapsible isExpanded={openPanel === "payment"}>
          <TonalBlock tone="muted">
            <PaymentPanel order={order} />
          </TonalBlock>
        </Collapsible>
      ) : null}
      {canManage && isSettled && hasRefundableLines ? (
        <Collapsible isExpanded={openPanel === "refund"}>
          <TonalBlock tone="muted">
            <RefundPanel order={order} onDone={() => setOpenPanel(null)} />
          </TonalBlock>
        </Collapsible>
      ) : null}
    </HairlineRow>
  );
}

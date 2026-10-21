import { useState } from "react";
import { View } from "react-native";
import { formatMoney } from "@/formatting/money";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { PaymentPanel } from "@/features/orders/components/PaymentPanel";
import { RefundPanel } from "@/features/orders/components/RefundPanel";
import { refundableLinesOf } from "@/features/orders/refunds";
import {
  isFullyRefunded,
  orderClientLabel,
  orderPaymentStatusLabel,
  orderStatusLabel,
} from "@/features/orders/orderLabels";
import { Order } from "@/features/orders/types";
import {
  useCancelOrder,
  useMarkOrderDelivered,
  useMarkOrderReady,
} from "@/features/orders/useOrderMutations";
import { formatLongDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
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
  if (order.status === "Cancelled" || isFullyRefunded(order)) {
    return "neutral";
  }
  return order.status === "Pending" ? "warning" : "success";
}

const paymentStatusTones: Record<Order["paymentStatus"], StatusTone> = {
  Unpaid: "danger",
  Partial: "warning",
  Paid: "success",
};

interface OrderActionProps {
  label: string;
  isDanger?: boolean;
  onPress: () => void;
  isLoading?: boolean;
  isExpanded?: boolean;
}

function OrderAction({
  label,
  isDanger = false,
  onPress,
  isLoading = false,
  isExpanded,
}: OrderActionProps) {
  return (
    <Button
      variant={isDanger ? "dangerGhost" : "ghost"}
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
  const business = useCurrentBusiness();
  const markOrderDeliveredMutation = useMarkOrderDelivered();
  const markOrderReadyMutation = useMarkOrderReady();
  const cancelOrderMutation = useCancelOrder();
  const [openPanel, setOpenPanel] = useState<OrderPanel | null>(null);
  const [isConfirmingCancel, setIsConfirmingCancel] = useState(false);
  const hasRefundableLines = refundableLinesOf(order).length > 0;
  const isPending = order.status === "Pending";
  const isOwing = order.status !== "Cancelled" && order.balance > 0;
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

  const cancel = async () => {
    try {
      await cancelOrderMutation.mutateAsync(order.id);
      showToast(translate("orders.cancelled"));
    } catch {
      showToast(translate("common.unexpectedError"));
    } finally {
      setIsConfirmingCancel(false);
    }
  };

  const amountToReturn = Math.min(order.paidAmount, order.total - order.refundedAmount);

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
          formatLongDate(order.lastPaidOn ?? order.createdAt.slice(0, isoDateLength)),
          order.channel === "App" ? translate("orders.fromApp") : null,
          lineSummary,
        ]
          .filter(Boolean)
          .join(" · ")}
      </AppText>
      {isOwing ? (
        <AppText variant="footnote" tone="subtle">
          {order.paidAmount > 0
            ? translate("orders.paidAndBalance", {
                paid: money(order.paidAmount),
                balance: money(order.balance),
              })
            : translate("orders.balance", { balance: money(order.balance) })}
        </AppText>
      ) : null}
      {order.returnedAmount > 0 ? (
        <AppText variant="footnote" tone="subtle">
          {translate("orders.refundedTotal", { amount: money(order.returnedAmount) })}
        </AppText>
      ) : null}
      {hasProducts && order.awaitsPickup ? (
        <AppText variant="footnote" tone="subtle">
          {deliveryLabel}
        </AppText>
      ) : null}
      <View className="mt-1 flex-row flex-wrap items-center justify-between gap-x-2 gap-y-1">
        <View className="flex-row flex-wrap items-center gap-1.5">
          <StatusPill label={orderStatusLabel(order)} tone={orderStatusTone(order)} isSmall />
          {order.status === "Cancelled" ? null : (
            <StatusPill
              label={orderPaymentStatusLabel(order)}
              tone={paymentStatusTones[order.paymentStatus]}
              isSmall
            />
          )}
          {order.awaitsPickup && order.readyAt !== null ? (
            <StatusPill label={translate("orders.readyPill")} tone="primary" isSmall />
          ) : null}
        </View>
        <View className="ml-auto max-w-full flex-row flex-wrap items-center justify-end">
          {canManage && isOwing ? (
            <OrderAction
              label={translate(openPanel === "payment" ? "common.close" : "orders.collect")}
              isExpanded={openPanel === "payment"}
              onPress={() => togglePanel("payment")}
            />
          ) : null}
          {canManage && order.awaitsPickup && order.readyAt === null ? (
            <OrderAction
              label={translate("orders.markReady")}
              onPress={markReady}
              isLoading={markOrderReadyMutation.isPending}
            />
          ) : null}
          {canManage && isPending ? (
            <OrderAction
              label={translate("orders.markDelivered")}
              onPress={markDelivered}
              isLoading={markOrderDeliveredMutation.isPending}
            />
          ) : null}
          {canManage && hasRefundableLines ? (
            <OrderAction
              label={translate(openPanel === "refund" ? "common.close" : "orders.refund.open")}
              isExpanded={openPanel === "refund"}
              onPress={() => togglePanel("refund")}
            />
          ) : null}
          {canManage && isPending ? (
            <OrderAction
              label={translate("orders.cancel")}
              isDanger
              onPress={() => setIsConfirmingCancel(true)}
            />
          ) : null}
        </View>
      </View>
      {canManage && isOwing ? (
        <Collapsible isExpanded={openPanel === "payment"}>
          <TonalBlock tone="muted">
            <PaymentPanel order={order} />
          </TonalBlock>
        </Collapsible>
      ) : null}
      {canManage && hasRefundableLines ? (
        <Collapsible isExpanded={openPanel === "refund"}>
          <TonalBlock tone="muted">
            <RefundPanel order={order} onDone={() => setOpenPanel(null)} />
          </TonalBlock>
        </Collapsible>
      ) : null}
      {isConfirmingCancel ? (
        <ConfirmDialog
          title={translate("orders.cancelConfirm.title")}
          message={
            amountToReturn > 0
              ? translate("orders.cancelConfirm.refund", {
                  amount: money(amountToReturn),
                  businessName: business.name,
                })
              : undefined
          }
          confirmLabel={translate("orders.cancel")}
          onCancel={() => setIsConfirmingCancel(false)}
          onConfirm={cancel}
          isConfirming={cancelOrderMutation.isPending}
        />
      ) : null}
    </HairlineRow>
  );
}

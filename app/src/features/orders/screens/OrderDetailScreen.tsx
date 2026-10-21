import { useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { permissions } from "@/features/members/permissions";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { CollectSheet } from "@/features/orders/components/CollectSheet";
import { OrderPaymentsPanel } from "@/features/orders/components/OrderPaymentsPanel";
import { RefundPanel } from "@/features/orders/components/RefundPanel";
import {
  orderClientLabel,
  orderDeliveryLabel,
  orderMoneyLabel,
  orderStatusLabel,
  orderStatusTone,
  orderSummaryLine,
} from "@/features/orders/orderLabels";
import { refundableLinesOf } from "@/features/orders/refunds";
import { Order } from "@/features/orders/types";
import { useOrderActions } from "@/features/orders/useOrderActions";
import { useCancelOrder } from "@/features/orders/useOrderMutations";
import { useOrder } from "@/features/orders/useOrders";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { HairlineRow } from "@/ui/HairlineRow";
import { HairlineSection } from "@/ui/HairlineSection";
import { ProgressBar } from "@/ui/ProgressBar";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { StatusPill } from "@/ui/StatusPill";
import { TonalBlock } from "@/ui/TonalBlock";
import { useToast } from "@/ui/ToastProvider";
import { Collapsible } from "@/ui/motion/Collapsible";
import { SkeletonScreen } from "@/ui/SkeletonScreen";

interface OrderDetailScreenProps {
  orderId: string;
}

function cancelQuestionOf(order: Order): string {
  return order.clientFullName === null
    ? translate("orders.cancelQuestion", { number: order.number })
    : translate("orders.cancelQuestionWithClient", {
        number: order.number,
        client: order.clientFullName,
      });
}

export function OrderDetailScreen({ orderId }: OrderDetailScreenProps) {
  const { data: order, isPending, isError, refetch } = useOrder(orderId);

  if (isPending) {
    return <SkeletonScreen />;
  }

  if (isError || order === undefined) {
    return (
      <ScrollScreen
        header={<ScreenHeader navigation="back" title={translate("orders.detail.title")} />}
      >
        <Banner tone="error" message={translate("orders.detail.loadError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={() => refetch()} />
        </Banner>
      </ScrollScreen>
    );
  }

  return <OrderDetail order={order} />;
}

function OrderDetail({ order }: { order: Order }) {
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const canManage = useCan(permissions.ordersManage);
  const { markDelivered, isDelivering, markReady, isMarkingReady } = useOrderActions(order);
  const cancelOrderMutation = useCancelOrder();
  const [isCollecting, setIsCollecting] = useState(false);
  const [isRefunding, setIsRefunding] = useState(false);
  const [isConfirmingCancel, setIsConfirmingCancel] = useState(false);
  const money = (amount: number) => formatMoney(amount, order.currencyCode);
  const isPending = order.status === "Pending";
  const isOwing = order.status !== "Cancelled" && order.balance > 0;
  const moneyLabel = orderMoneyLabel(order);
  const hasRefundableLines = refundableLinesOf(order).length > 0;
  const amountToReturn = Math.min(order.paidAmount, order.total - order.refundedAmount);

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

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={translate("orders.detail.title")}
          title={orderClientLabel(order)}
          subtitle={orderSummaryLine(order)}
        />
      }
      footer={
        canManage && isOwing ? (
          <Button
            icon="paid"
            label={translate("orders.collect")}
            onPress={() => setIsCollecting(true)}
          />
        ) : undefined
      }
    >
      <HairlineSection title={translate("orders.counterSale.items")}>
        {order.lines.map((line) => (
          <HairlineRow key={line.id} className="gap-0.5 py-2.5">
            <View className="flex-row items-center justify-between gap-3">
              <AppText variant="body" className="min-w-0 flex-1">
                {translate("orders.counterSale.lineQuantity", {
                  quantity: line.quantity,
                  name: line.name,
                })}
              </AppText>
              <AppText variant="bodyStrong">{money(line.total)}</AppText>
            </View>
            {line.refundedAmount > 0 ? (
              <AppText variant="footnote" tone="subtle">
                {translate("orders.refundedTotal", { amount: money(line.refundedAmount) })}
              </AppText>
            ) : null}
          </HairlineRow>
        ))}
        <View className="flex-row items-center justify-between gap-3 py-2.5">
          <AppText variant="bodyStrong">{translate("common.total")}</AppText>
          <AppText variant="bodyStrong">{money(order.total)}</AppText>
        </View>
      </HairlineSection>

      <HairlineSection title={translate("orders.detail.delivery")}>
        <View className="flex-row flex-wrap items-center gap-1.5 py-2">
          <StatusPill label={orderStatusLabel(order)} tone={orderStatusTone(order)} isSmall />
          {order.awaitsPickup && order.readyAt !== null ? (
            <StatusPill label={translate("orders.readyPill")} tone="primary" isSmall />
          ) : null}
        </View>
        {order.awaitsPickup ? (
          <AppText variant="footnote" tone="subtle">
            {orderDeliveryLabel(order)}
          </AppText>
        ) : null}
        {canManage && isPending ? (
          <View className="flex-row flex-wrap gap-2 pt-2">
            {order.awaitsPickup && order.readyAt === null ? (
              <Button
                variant="secondary"
                size="medium"
                label={translate("orders.markReady")}
                onPress={markReady}
                isLoading={isMarkingReady}
              />
            ) : null}
            <Button
              variant="secondary"
              size="medium"
              label={translate("orders.markDelivered")}
              onPress={markDelivered}
              isLoading={isDelivering}
            />
          </View>
        ) : null}
      </HairlineSection>

      <HairlineSection title={translate("orders.payments.open")}>
        {moneyLabel === null ? null : (
          <AppText variant="bodyStrong" tone={moneyLabel.tone} className="pt-2">
            {moneyLabel.text}
          </AppText>
        )}
        {isOwing && order.paidAmount > 0 ? (
          <ProgressBar ratio={order.paidAmount / (order.paidAmount + order.balance)} isThin />
        ) : null}
        <OrderPaymentsPanel order={order} canDelete={canManage && order.status !== "Cancelled"} />
      </HairlineSection>

      {canManage && (hasRefundableLines || isPending) ? (
        <View className="gap-2 border-t border-border-subtle pt-4">
          {hasRefundableLines ? (
            <Button
              variant="secondary"
              size="medium"
              label={translate(isRefunding ? "common.close" : "orders.refund.open")}
              isExpanded={isRefunding}
              onPress={() => setIsRefunding(!isRefunding)}
            />
          ) : null}
          {hasRefundableLines ? (
            <Collapsible isExpanded={isRefunding}>
              <TonalBlock tone="muted">
                <RefundPanel order={order} onDone={() => setIsRefunding(false)} />
              </TonalBlock>
            </Collapsible>
          ) : null}
          {isPending ? (
            <Button
              variant="dangerGhost"
              size="medium"
              label={translate("orders.cancel")}
              onPress={() => setIsConfirmingCancel(true)}
            />
          ) : null}
        </View>
      ) : null}

      {isCollecting ? <CollectSheet order={order} onClose={() => setIsCollecting(false)} /> : null}
      {isConfirmingCancel ? (
        <ConfirmDialog
          title={cancelQuestionOf(order)}
          message={[
            amountToReturn > 0
              ? translate("orders.cancelConfirm.refund", {
                  amount: money(amountToReturn),
                  businessName: business.name,
                })
              : null,
            translate("orders.cancelMessage"),
          ]
            .filter(Boolean)
            .join(" ")}
          confirmLabel={translate("orders.cancel")}
          onCancel={() => setIsConfirmingCancel(false)}
          onConfirm={cancel}
          isConfirming={cancelOrderMutation.isPending}
        />
      ) : null}
    </ScrollScreen>
  );
}

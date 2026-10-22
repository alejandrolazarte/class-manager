import { View } from "react-native";
import { formatLongDate } from "@/dates/dates";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";
import { Order, OrderPayment } from "@/features/orders/types";
import { useDeleteOrderPayment, useRestoreOrderPayment } from "@/features/orders/useOrderMutations";
import { permissions } from "@/features/members/permissions";
import { formatMoney } from "@/formatting/money";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconButton } from "@/ui/IconButton";
import { useToast } from "@/ui/ToastProvider";

interface OrderPaymentsPanelProps {
  order: Order;
  canDelete: boolean;
}

export function OrderPaymentsPanel({ order, canDelete }: OrderPaymentsPanelProps) {
  const { showToast } = useToast();
  const canUndoPayment = useCanUndoCollection(permissions.ordersManage);
  const deleteOrderPaymentMutation = useDeleteOrderPayment();
  const restoreOrderPaymentMutation = useRestoreOrderPayment();
  const money = (amount: number) => formatMoney(amount, order.currencyCode);

  const deletePayment = async (payment: OrderPayment) => {
    const paymentKey = { orderId: order.id, paymentId: payment.id };
    try {
      await deleteOrderPaymentMutation.mutateAsync(paymentKey);
      showToast(translate("orders.payments.deleted"), {
        label: translate("common.undo"),
        onPress: () => {
          restoreOrderPaymentMutation
            .mutateAsync(paymentKey)
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <View>
      {order.payments.map((payment) => (
        <HairlineRow key={payment.id} className="flex-row items-center gap-2 py-2">
          <View className="min-w-0 flex-1">
            <AppText variant="body">
              {translate(`fees.methods.${payment.method}` as TranslationKey)}
            </AppText>
            <AppText variant="footnote" tone="subtle">
              {formatLongDate(payment.paidOn)}
            </AppText>
          </View>
          <AppText variant="bodyStrong" tone="success">
            {money(payment.amount)}
          </AppText>
          {canDelete && canUndoPayment(payment.recordedByUserId) ? (
            <IconButton
              icon="delete"
              tone="muted-foreground"
              accessibilityLabel={translate("orders.payments.deleteOf", {
                amount: money(payment.amount),
              })}
              disabled={deleteOrderPaymentMutation.isPending}
              onPress={() => deletePayment(payment)}
            />
          ) : null}
        </HairlineRow>
      ))}
    </View>
  );
}

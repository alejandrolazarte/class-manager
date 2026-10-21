import { useState } from "react";
import { View } from "react-native";
import { AmountField } from "@/features/fees/components/AmountField";
import { PaymentMethodPicker } from "@/features/fees/components/PaymentMethodPicker";
import { PaymentMethod } from "@/features/fees/types";
import { paidAmountOf, paymentAmountError } from "@/features/orders/orderPayments";
import { Order } from "@/features/orders/types";
import { useRecordOrderPayment } from "@/features/orders/useOrderMutations";
import { formatMoney, toAmountText } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

interface PaymentPanelProps {
  order: Order;
}

export function PaymentPanel({ order }: PaymentPanelProps) {
  const { showToast } = useToast();
  const recordOrderPaymentMutation = useRecordOrderPayment();
  const [typedAmount, setTypedAmount] = useState(toAmountText(order.balance));
  const [method, setMethod] = useState<PaymentMethod>("Cash");
  const [hasFailed, setHasFailed] = useState(false);
  const hasProducts = order.lines.some((line) => line.kind === "Product");
  const isPending = order.status === "Pending";
  const [isDelivered, setIsDelivered] = useState(!hasProducts);
  const [isReady, setIsReady] = useState(true);
  const amount = paidAmountOf(typedAmount);
  const amountError = paymentAmountError(typedAmount, order.balance);

  const pay = async () => {
    setHasFailed(false);
    try {
      await recordOrderPaymentMutation.mutateAsync({
        orderId: order.id,
        request: {
          amount: amount ?? 0,
          method,
          isDelivered: isPending && isDelivered,
          isReady: isPending && hasProducts && !isDelivered && isReady,
        },
      });
      showToast(translate("orders.paid"));
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <View className="gap-2.5">
      <AmountField
        label={translate("orders.payment.amount")}
        isRequired
        currencyCode={order.currencyCode}
        value={typedAmount}
        onChangeText={setTypedAmount}
        errorMessage={
          amountError === null
            ? undefined
            : translate(amountError, { balance: formatMoney(order.balance, order.currencyCode) })
        }
      />
      <PaymentMethodPicker value={method} onChange={setMethod} />
      {isPending ? (
        <ToggleSwitch
          icon="handOver"
          label={translate("orders.counterSale.handedOver")}
          hint={translate(
            isDelivered
              ? "orders.counterSale.handedOverHint"
              : "orders.counterSale.pendingHandOverHint",
          )}
          value={isDelivered}
          onValueChange={setIsDelivered}
        />
      ) : null}
      {isPending && hasProducts && !isDelivered && order.readyAt === null ? (
        <ToggleSwitch
          label={translate("orders.isReadyNow")}
          value={isReady}
          onValueChange={setIsReady}
        />
      ) : null}
      {hasFailed ? <Banner tone="warning" message={translate("common.unexpectedError")} /> : null}
      <Button
        size="medium"
        icon="paid"
        label={translate("orders.confirmPayment")}
        disabled={amount === null || amountError !== null}
        onPress={pay}
        isLoading={recordOrderPaymentMutation.isPending}
      />
    </View>
  );
}

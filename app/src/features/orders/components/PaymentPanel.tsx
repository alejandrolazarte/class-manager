import { useState } from "react";
import { View } from "react-native";
import { PaymentMethodPicker } from "@/features/fees/components/PaymentMethodPicker";
import { PaymentMethod } from "@/features/fees/types";
import { Order } from "@/features/orders/types";
import { useCancelOrder, useConfirmOrderPayment } from "@/features/orders/useOrderMutations";
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
  const confirmOrderPaymentMutation = useConfirmOrderPayment();
  const cancelOrderMutation = useCancelOrder();
  const [method, setMethod] = useState<PaymentMethod>("Cash");
  const [hasFailed, setHasFailed] = useState(false);
  const hasProducts = order.lines.some((line) => line.kind === "Product");
  const [isReady, setIsReady] = useState(true);

  const run = async (
    action: () => Promise<unknown>,
    successKey: "orders.paid" | "orders.cancelled",
  ) => {
    setHasFailed(false);
    try {
      await action();
      showToast(translate(successKey));
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <View className="gap-2.5">
      <PaymentMethodPicker value={method} onChange={setMethod} />
      {hasProducts ? (
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
        onPress={() =>
          run(
            () =>
              confirmOrderPaymentMutation.mutateAsync({
                orderId: order.id,
                method,
                isReady: hasProducts && isReady,
              }),
            "orders.paid",
          )
        }
        isLoading={confirmOrderPaymentMutation.isPending}
      />
      <Button
        size="medium"
        variant="dangerOutline"
        label={translate("orders.cancel")}
        onPress={() => run(() => cancelOrderMutation.mutateAsync(order.id), "orders.cancelled")}
        isLoading={cancelOrderMutation.isPending}
      />
    </View>
  );
}

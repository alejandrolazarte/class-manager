import { Order } from "@/features/orders/types";
import { useMarkOrderDelivered, useMarkOrderReady } from "@/features/orders/useOrderMutations";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

export function useOrderActions(order: Order) {
  const { showToast } = useToast();
  const markOrderDeliveredMutation = useMarkOrderDelivered();
  const markOrderReadyMutation = useMarkOrderReady();

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

  return {
    markDelivered,
    isDelivering: markOrderDeliveredMutation.isPending,
    markReady,
    isMarkingReady: markOrderReadyMutation.isPending,
  };
}

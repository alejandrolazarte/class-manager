import { Order } from "@/features/orders/types";
import { translate, TranslationKey } from "@/i18n/translate";

export function orderStatusLabel(order: Order): string {
  if (order.awaitsPickup) {
    return translate("orders.status.awaitingPickup");
  }
  if (order.refundedAmount > 0 && order.refundedAmount >= order.total) {
    return translate("orders.status.refunded");
  }
  return translate(`orders.status.${order.status}` as TranslationKey);
}

export function orderClientLabel(order: Order): string {
  return order.clientFullName ?? translate("orders.noFamily");
}

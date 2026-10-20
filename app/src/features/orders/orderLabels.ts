import { Order, OrderPaymentStatus } from "@/features/orders/types";
import { translate, TranslationKey } from "@/i18n/translate";

const paymentStatusLabels: Record<OrderPaymentStatus, TranslationKey> = {
  Unpaid: "orders.paymentStatus.Unpaid",
  Partial: "orders.paymentStatus.Partial",
  Paid: "orders.paymentStatus.Paid",
};

export function isFullyRefunded(order: Order): boolean {
  return order.refundedAmount > 0 && order.refundedAmount >= order.total;
}

export function orderStatusLabel(order: Order): string {
  if (order.status === "Cancelled") {
    return translate("orders.status.Cancelled");
  }
  if (isFullyRefunded(order)) {
    return translate("orders.status.refunded");
  }
  if (order.awaitsPickup) {
    return translate("orders.status.awaitingPickup");
  }
  return translate(
    order.status === "Delivered" ? "orders.status.Delivered" : "orders.status.Pending",
  );
}

export function orderPaymentStatusLabel(order: Order): string {
  return translate(paymentStatusLabels[order.paymentStatus]);
}

export function orderClientLabel(order: Order): string {
  return order.clientFullName ?? translate("orders.noClient");
}

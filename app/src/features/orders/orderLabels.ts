import { formatLongDate } from "@/dates/dates";
import { formatMoney } from "@/formatting/money";
import { Order } from "@/features/orders/types";
import { translate } from "@/i18n/translate";
import { TextTone } from "@/ui/AppText";
import { StatusTone } from "@/ui/StatusPill";

const isoDateLength = 10;

export interface OrderMoneyLabel {
  text: string;
  tone: TextTone;
}

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

export function orderMoneyLabel(order: Order): OrderMoneyLabel | null {
  const money = (amount: number) => formatMoney(amount, order.currencyCode);
  if (order.status === "Cancelled" || isFullyRefunded(order)) {
    return order.returnedAmount > 0
      ? {
          text: translate("orders.refundedTotal", { amount: money(order.returnedAmount) }),
          tone: "subtle",
        }
      : null;
  }
  if (order.balance > 0) {
    return {
      text:
        order.paidAmount > 0
          ? translate("orders.paidAndBalance", {
              paid: money(order.paidAmount),
              balance: money(order.balance),
            })
          : translate("orders.balance", { balance: money(order.balance) }),
      tone: "warning",
    };
  }
  return { text: translate("orders.paymentStatus.Paid"), tone: "success" };
}

export function orderClientLabel(order: Order): string {
  return order.clientFullName ?? translate("orders.noClient");
}

export function orderStatusTone(order: Order): StatusTone {
  if (order.status === "Cancelled" || isFullyRefunded(order)) {
    return "neutral";
  }
  return order.status === "Pending" ? "warning" : "success";
}

export function orderSummaryLine(order: Order): string {
  const lineSummary = order.lines
    .map((line) => (line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name))
    .join(", ");
  return [
    translate("orders.number", { number: order.number }),
    formatLongDate(order.lastPaidOn ?? order.createdAt.slice(0, isoDateLength)),
    order.channel === "App" ? translate("orders.fromApp") : null,
    lineSummary,
  ]
    .filter(Boolean)
    .join(" · ");
}

export function orderDeliveryLabel(order: Order): string {
  return order.delivery === "InClass" && order.deliveryClassGroupName !== null
    ? translate("orders.deliveryInClass", { className: order.deliveryClassGroupName })
    : translate("orders.deliveryPickup");
}

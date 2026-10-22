import { Order, OrderLine } from "@/features/orders/types";

export function remainingUnits(line: OrderLine): number {
  return line.quantity - line.refundedQuantity;
}

function isRefundable(line: OrderLine): boolean {
  return remainingUnits(line) > 0 && (line.kind === "Product" || line.classPackPurchaseId !== null);
}

export function refundableLinesOf(order: Order): OrderLine[] {
  const acceptsRefunds =
    order.status === "Delivered" || (order.status === "Pending" && order.payments.length > 0);
  return acceptsRefunds ? order.lines.filter(isRefundable) : [];
}

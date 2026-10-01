import { Order, OrderLine } from "@/features/orders/types";

export function remainingUnits(line: OrderLine): number {
  return line.quantity - line.refundedQuantity;
}

export function refundableLinesOf(order: Order): OrderLine[] {
  return order.lines.filter((line) => remainingUnits(line) > 0);
}

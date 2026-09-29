import { httpClient } from "@/api/httpClient";
import { PaymentMethod } from "@/features/fees/types";
import { CreateCounterSaleRequest, Order, OrderFilter, RefundLine } from "@/features/orders/types";

const ordersPath = "/api/orders";
const deliveredSegment = "delivered";
const refundsSegment = "refunds";
const paymentSegment = "payment";
const cancellationSegment = "cancellation";

function orderPath(orderId: string): string {
  return `${ordersPath}/${encodeURIComponent(orderId)}`;
}

export function listOrders(clientId: string | null, filter: OrderFilter): Promise<Order[]> {
  const queryParameters: Record<string, string> = {
    awaitingPickup: String(filter === "awaitingPickup"),
    requested: String(filter === "requested"),
  };
  if (clientId !== null) {
    queryParameters.clientId = clientId;
  }
  return httpClient.get<Order[]>(ordersPath, queryParameters);
}

export function createCounterSale(request: CreateCounterSaleRequest): Promise<Order> {
  return httpClient.post<Order>(ordersPath, request);
}

export function markOrderDelivered(orderId: string): Promise<Order> {
  return httpClient.put<Order>(`${orderPath(orderId)}/${deliveredSegment}`, {});
}

export function refundOrder(orderId: string, lines: RefundLine[]): Promise<Order> {
  return httpClient.post<Order>(`${orderPath(orderId)}/${refundsSegment}`, { lines });
}

export function confirmOrderPayment(orderId: string, method: PaymentMethod): Promise<Order> {
  return httpClient.put<Order>(`${orderPath(orderId)}/${paymentSegment}`, {
    method,
    paidOn: null,
  });
}

export function cancelOrder(orderId: string): Promise<Order> {
  return httpClient.put<Order>(`${orderPath(orderId)}/${cancellationSegment}`, {});
}

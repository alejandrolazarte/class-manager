import { httpClient } from "@/api/httpClient";
import { CreateCounterSaleRequest, Order, RefundLine } from "@/features/orders/types";

const ordersPath = "/api/orders";
const deliveredSegment = "delivered";
const refundsSegment = "refunds";

function orderPath(orderId: string): string {
  return `${ordersPath}/${encodeURIComponent(orderId)}`;
}

export function listOrders(clientId: string | null, awaitingPickup: boolean): Promise<Order[]> {
  const queryParameters: Record<string, string> = { awaitingPickup: String(awaitingPickup) };
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

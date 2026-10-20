import { httpClient } from "@/api/httpClient";
import { DeliveryClass } from "@/features/studentApp/types";
import {
  ClassDelivery,
  CreateCounterSaleRequest,
  Order,
  OrderFilter,
  OrderSummary,
  RecordOrderPaymentRequest,
  RefundLine,
} from "@/features/orders/types";

const ordersPath = "/api/orders";
const summaryPath = `${ordersPath}/summary`;
const deliveredSegment = "delivered";
const refundsSegment = "refunds";
const paymentsSegment = "payments";
const cancellationSegment = "cancellation";
const readySegment = "ready";
const deliveryClassesPath = `${ordersPath}/delivery-classes`;
const classGroupsPath = "/api/class-groups";
const deliveriesSegment = "deliveries";

function orderPath(orderId: string): string {
  return `${ordersPath}/${encodeURIComponent(orderId)}`;
}

export function listOrders(clientId: string | null, filter: OrderFilter): Promise<Order[]> {
  const queryParameters: Record<string, string> = {
    awaitingPickup: String(filter === "awaitingPickup"),
    owing: String(filter === "owing"),
  };
  if (clientId !== null) {
    queryParameters.clientId = clientId;
  }
  return httpClient.get<Order[]>(ordersPath, queryParameters);
}

export function getOrderSummary(month: string): Promise<OrderSummary> {
  return httpClient.get<OrderSummary>(summaryPath, { month });
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

export function recordOrderPayment(
  orderId: string,
  request: RecordOrderPaymentRequest,
): Promise<Order> {
  return httpClient.post<Order>(`${orderPath(orderId)}/${paymentsSegment}`, {
    ...request,
    paidOn: null,
  });
}

export function markOrderReady(orderId: string): Promise<Order> {
  return httpClient.put<Order>(`${orderPath(orderId)}/${readySegment}`, {});
}

export function listDeliveryClasses(clientId: string): Promise<DeliveryClass[]> {
  return httpClient.get<DeliveryClass[]>(deliveryClassesPath, { clientId });
}

function classDeliveriesPath(classGroupId: string): string {
  return `${classGroupsPath}/${encodeURIComponent(classGroupId)}/${deliveriesSegment}`;
}

export function listClassDeliveries(classGroupId: string): Promise<ClassDelivery[]> {
  return httpClient.get<ClassDelivery[]>(classDeliveriesPath(classGroupId));
}

export function deliverInClass(classGroupId: string, orderId: string): Promise<ClassDelivery> {
  return httpClient.put<ClassDelivery>(
    `${classDeliveriesPath(classGroupId)}/${encodeURIComponent(orderId)}`,
    {},
  );
}

export function cancelOrder(orderId: string): Promise<Order> {
  return httpClient.put<Order>(`${orderPath(orderId)}/${cancellationSegment}`, {});
}

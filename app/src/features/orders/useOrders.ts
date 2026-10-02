import { useQuery } from "@tanstack/react-query";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";
import { listClassDeliveries, listDeliveryClasses, listOrders } from "@/features/orders/ordersApi";
import { Order, OrderFilter } from "@/features/orders/types";

export function useOrders(clientId: string | null, filter: OrderFilter) {
  return useQuery({
    queryKey: orderQueryKeys.list(clientId, filter),
    queryFn: () => listOrders(clientId, filter),
  });
}

export function useClassDeliveries(classGroupId: string, { enabled = true } = {}) {
  return useQuery({
    queryKey: orderQueryKeys.classDeliveries(classGroupId),
    queryFn: () => listClassDeliveries(classGroupId),
    enabled,
  });
}

export function useDeliveryClasses(clientId: string | undefined) {
  return useQuery({
    queryKey: orderQueryKeys.deliveryClasses(clientId ?? ""),
    queryFn: () => listDeliveryClasses(clientId ?? ""),
    enabled: clientId !== undefined,
  });
}

export interface PendingOrders {
  requested: Order[];
  awaitingPickup: Order[];
  count: number;
}

export function usePendingOrders({ enabled = true } = {}): PendingOrders {
  const { data: requested = [] } = useQuery({
    queryKey: orderQueryKeys.list(null, "requested"),
    queryFn: () => listOrders(null, "requested"),
    enabled,
  });
  const { data: awaitingPickup = [] } = useQuery({
    queryKey: orderQueryKeys.list(null, "awaitingPickup"),
    queryFn: () => listOrders(null, "awaitingPickup"),
    enabled,
  });
  return { requested, awaitingPickup, count: requested.length + awaitingPickup.length };
}

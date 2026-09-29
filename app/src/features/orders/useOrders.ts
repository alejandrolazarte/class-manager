import { useQuery } from "@tanstack/react-query";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";
import { listClassDeliveries, listDeliveryClasses, listOrders } from "@/features/orders/ordersApi";
import { OrderFilter } from "@/features/orders/types";

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

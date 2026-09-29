import { useQuery } from "@tanstack/react-query";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";
import { listOrders } from "@/features/orders/ordersApi";

export function useOrders(clientId: string | null, awaitingPickup: boolean) {
  return useQuery({
    queryKey: orderQueryKeys.list(clientId, awaitingPickup),
    queryFn: () => listOrders(clientId, awaitingPickup),
  });
}

import { useQuery } from "@tanstack/react-query";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderFilter } from "@/features/orders/types";

export function useOrders(clientId: string | null, filter: OrderFilter) {
  return useQuery({
    queryKey: orderQueryKeys.list(clientId, filter),
    queryFn: () => listOrders(clientId, filter),
  });
}

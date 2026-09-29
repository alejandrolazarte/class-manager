import { OrderFilter } from "@/features/orders/types";

export const orderQueryKeys = {
  all: ["orders"] as const,
  list: (clientId: string | null, filter: OrderFilter) =>
    [...orderQueryKeys.all, "list", clientId, filter] as const,
};

import { OrderFilter } from "@/features/orders/types";

export const orderQueryKeys = {
  all: ["orders"] as const,
  list: (clientId: string | null, filter: OrderFilter) =>
    [...orderQueryKeys.all, "list", clientId, filter] as const,
  summary: (month: string) => [...orderQueryKeys.all, "summary", month] as const,
  classDeliveries: (classGroupId: string) =>
    [...orderQueryKeys.all, "classDeliveries", classGroupId] as const,
  deliveryClasses: (clientId: string) =>
    [...orderQueryKeys.all, "deliveryClasses", clientId] as const,
};

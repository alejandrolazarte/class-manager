import { PaymentMethod } from "@/features/fees/types";

export type OrderStatus = "Requested" | "Paid" | "Delivered" | "Cancelled";

export type OrderFilter = "all" | "requested" | "awaitingPickup";

export const orderFilters: readonly OrderFilter[] = ["all", "requested", "awaitingPickup"];

export type OrderLineKind = "ClassPack" | "Product";

export interface OrderLine {
  id: string;
  kind: OrderLineKind;
  name: string;
  quantity: number;
  unitPrice: number;
  total: number;
  refundedQuantity: number;
  refundedAmount: number;
  classPackPurchaseId: string | null;
}

export interface Order {
  id: string;
  clientId: string | null;
  clientFullName: string | null;
  channel: "Counter" | "App";
  status: OrderStatus;
  awaitsPickup: boolean;
  method: PaymentMethod | null;
  paidOn: string | null;
  total: number;
  refundedAmount: number;
  notes: string | null;
  createdAt: string;
  deliveredAt: string | null;
  lines: OrderLine[];
  cancelledAt: string | null;
}

export interface CounterSaleLine {
  classPackId: string | null;
  productVariantId: string | null;
  quantity: number | null;
  unitPrice: number | null;
}

export interface CreateCounterSaleRequest {
  clientId: string | null;
  lines: CounterSaleLine[];
  method: PaymentMethod;
  paidOn: string | null;
  notes: string | null;
  isDelivered: boolean;
}

export interface RefundLine {
  lineId: string;
  quantity: number | null;
  returnToStock: boolean;
}

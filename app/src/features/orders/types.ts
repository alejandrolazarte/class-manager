import { PaymentMethod } from "@/features/fees/types";

export type OrderStatus = "Pending" | "Delivered" | "Cancelled";

export type OrderPaymentStatus = "Unpaid" | "Partial" | "Paid";

export type DeliveryMethod = "Pickup" | "InClass";

export interface ClassDelivery {
  orderId: string;
  clientFullName: string | null;
  isReady: boolean;
  lines: { name: string; quantity: number }[];
}

export type OrderFilter = "all" | "owing" | "awaitingPickup";

export const orderFilters: readonly OrderFilter[] = ["all", "owing", "awaitingPickup"];

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

export interface OrderPayment {
  id: string;
  amount: number;
  method: PaymentMethod;
  paidOn: string;
  recordedByUserId: string | null;
}

export interface Order {
  id: string;
  number: number;
  clientId: string | null;
  clientFullName: string | null;
  channel: "Counter" | "App";
  status: OrderStatus;
  paymentStatus: OrderPaymentStatus;
  awaitsPickup: boolean;
  lastPaidOn: string | null;
  currencyCode: string;
  total: number;
  refundedAmount: number;
  paidAmount: number;
  balance: number;
  returnedAmount: number;
  payments: OrderPayment[];
  notes: string | null;
  createdAt: string;
  deliveredAt: string | null;
  lines: OrderLine[];
  cancelledAt: string | null;
  delivery: DeliveryMethod;
  deliveryClassGroupId: string | null;
  deliveryClassGroupName: string | null;
  readyAt: string | null;
}

export interface OrderSummary {
  month: string;
  collected: number;
  unpaid: number;
  classPackSales: number;
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
  paidAmount: number;
  method: PaymentMethod;
  paidOn: string | null;
  notes: string | null;
  isDelivered: boolean;
  delivery: DeliveryMethod | null;
  deliveryClassGroupId: string | null;
}

export interface RecordOrderPaymentRequest {
  amount: number;
  method: PaymentMethod;
  isDelivered: boolean;
  isReady: boolean;
}

export interface RefundLine {
  lineId: string;
  quantity: number | null;
  returnToStock: boolean;
}

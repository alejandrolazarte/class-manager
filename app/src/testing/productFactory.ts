import { Order, OrderSummary } from "@/features/orders/types";
import { Product } from "@/features/products/types";

export function buildProduct(overrides: Partial<Product> = {}): Product {
  return {
    id: "0192f0c4-0000-7000-8000-00000000b001",
    name: "Gorro de natación",
    description: null,
    price: 12,
    stockMode: "Tracked",
    isVisibleInApp: true,
    isActive: true,
    variants: [{ id: "0192f0c4-0000-7000-8000-00000000b0a1", name: "", stock: 3 }],
    imageUrl: null,
    ...overrides,
  };
}

export function buildOrder(overrides: Partial<Order> = {}): Order {
  return {
    id: "0192f0c4-0000-7000-8000-00000000d001",
    number: 1043,
    clientId: null,
    clientFullName: null,
    channel: "Counter",
    status: "Paid",
    awaitsPickup: true,
    method: "Cash",
    paidOn: "2026-09-29",
    total: 24,
    refundedAmount: 0,
    notes: null,
    createdAt: "2026-09-29T10:00:00Z",
    deliveredAt: null,
    cancelledAt: null,
    delivery: "Pickup",
    deliveryClassGroupId: null,
    deliveryClassGroupName: null,
    readyAt: null,
    lines: [
      {
        id: "0192f0c4-0000-7000-8000-00000000d0a1",
        kind: "Product",
        name: "Gorro de natación",
        quantity: 2,
        unitPrice: 12,
        total: 24,
        refundedQuantity: 0,
        refundedAmount: 0,
        classPackPurchaseId: null,
      },
    ],
    ...overrides,
  };
}

export function buildOrderSummary(overrides: Partial<OrderSummary> = {}): OrderSummary {
  return {
    month: "2026-09",
    collected: 0,
    unpaid: 0,
    classPackSales: 0,
    ...overrides,
  };
}

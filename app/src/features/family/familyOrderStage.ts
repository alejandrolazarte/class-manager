import { StatusTone } from "@/ui/StatusPill";
import { FamilyOrder } from "@/features/family/types";
import { TranslationKey } from "@/i18n/translate";

export type FamilyOrderStage =
  "awaitingPayment" | "preparing" | "ready" | "paid" | "delivered" | "cancelled";

export type FamilyOrderFilter = "open" | "past" | "all";

export const familyOrderFilters: readonly FamilyOrderFilter[] = ["open", "past", "all"];

export function orderStage(order: FamilyOrder): FamilyOrderStage {
  if (order.status === "Requested") {
    return "awaitingPayment";
  }
  if (order.status === "Cancelled") {
    return "cancelled";
  }
  if (order.awaitsPickup) {
    return order.isReady ? "ready" : "preparing";
  }
  return order.status === "Delivered" ? "delivered" : "paid";
}

const openStages: readonly FamilyOrderStage[] = ["awaitingPayment", "preparing", "ready"];

export function isOrderOpen(order: FamilyOrder): boolean {
  return openStages.includes(orderStage(order));
}

export function matchesOrderFilter(order: FamilyOrder, filter: FamilyOrderFilter): boolean {
  if (filter === "all") {
    return true;
  }
  return isOrderOpen(order) === (filter === "open");
}

export const orderStageLabels: Record<FamilyOrderStage, TranslationKey> = {
  awaitingPayment: "family.orders.status.Requested",
  preparing: "family.orders.status.preparing",
  ready: "family.orders.status.ready",
  paid: "family.orders.status.Paid",
  delivered: "family.orders.status.Delivered",
  cancelled: "family.orders.status.Cancelled",
};

export const orderStageTones: Record<FamilyOrderStage, StatusTone> = {
  awaitingPayment: "warning",
  preparing: "primary",
  ready: "success",
  paid: "success",
  delivered: "neutral",
  cancelled: "danger",
};

export const orderSteps: readonly TranslationKey[] = [
  "family.orders.step.ordered",
  "family.orders.step.paid",
  "family.orders.step.ready",
  "family.orders.step.delivered",
];

const stageStepIndexes: Partial<Record<FamilyOrderStage, number>> = {
  awaitingPayment: 0,
  preparing: 1,
  ready: 2,
  delivered: 3,
};

export function orderStepIndex(order: FamilyOrder): number | null {
  const hasProducts = order.lines.some((line) => line.kind === "Product");
  return hasProducts ? (stageStepIndexes[orderStage(order)] ?? null) : null;
}

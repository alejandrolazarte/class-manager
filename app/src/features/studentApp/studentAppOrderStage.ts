import { StatusTone } from "@/ui/StatusPill";
import { StudentAppOrder } from "@/features/studentApp/types";
import { TranslationKey } from "@/i18n/translate";

export type StudentAppOrderStage =
  "awaitingPayment" | "preparing" | "ready" | "paid" | "delivered" | "cancelled";

export type StudentAppOrderFilter = "open" | "past" | "all";

export const studentAppOrderFilters: readonly StudentAppOrderFilter[] = ["open", "past", "all"];

export function orderStage(order: StudentAppOrder): StudentAppOrderStage {
  if (order.status === "Cancelled") {
    return "cancelled";
  }
  if (order.status === "Delivered") {
    return "delivered";
  }
  if (order.paymentStatus !== "Paid") {
    return "awaitingPayment";
  }
  if (order.awaitsPickup) {
    return order.isReady ? "ready" : "preparing";
  }
  return "paid";
}

const openStages: readonly StudentAppOrderStage[] = ["awaitingPayment", "preparing", "ready"];

export function isOrderOpen(order: StudentAppOrder): boolean {
  return openStages.includes(orderStage(order));
}

export function matchesOrderFilter(order: StudentAppOrder, filter: StudentAppOrderFilter): boolean {
  if (filter === "all") {
    return true;
  }
  return isOrderOpen(order) === (filter === "open");
}

export const orderStageLabels: Record<StudentAppOrderStage, TranslationKey> = {
  awaitingPayment: "student.orders.status.awaitingPayment",
  preparing: "student.orders.status.preparing",
  ready: "student.orders.status.ready",
  paid: "student.orders.status.paid",
  delivered: "student.orders.status.Delivered",
  cancelled: "student.orders.status.Cancelled",
};

export const orderStageTones: Record<StudentAppOrderStage, StatusTone> = {
  awaitingPayment: "warning",
  preparing: "primary",
  ready: "success",
  paid: "success",
  delivered: "neutral",
  cancelled: "danger",
};

export const orderSteps: readonly TranslationKey[] = [
  "student.orders.step.ordered",
  "student.orders.step.paid",
  "student.orders.step.ready",
  "student.orders.step.delivered",
];

const stageStepIndexes: Partial<Record<StudentAppOrderStage, number>> = {
  awaitingPayment: 0,
  preparing: 1,
  ready: 2,
  delivered: 3,
};

export function orderStepIndex(order: StudentAppOrder): number | null {
  const hasProducts = order.lines.some((line) => line.kind === "Product");
  return hasProducts ? (stageStepIndexes[orderStage(order)] ?? null) : null;
}

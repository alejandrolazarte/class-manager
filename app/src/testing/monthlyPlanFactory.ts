import { ClassPack } from "@/features/classPacks/types";
import { buildClassPack } from "@/testing/classPackFactory";

export function buildMonthlyPlan(overrides: Partial<ClassPack> = {}): ClassPack {
  return buildClassPack({
    id: "0192f0c4-0000-7000-8000-00000000c1a6",
    name: "Cuota mensual",
    classCount: null,
    validityMonths: null,
    renewsMonthly: true,
    appliesFamilyDiscount: true,
    coversAnyGroupClass: true,
    price: 25000,
    ...overrides,
  });
}

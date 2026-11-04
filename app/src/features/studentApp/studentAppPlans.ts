import { StudentAppPlanStudent, StudentAppShopPlan } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";

export type PlanStudentStatus =
  | { kind: "active" }
  | { kind: "ending" }
  | { kind: "starting" }
  | { kind: "other"; planName: string }
  | { kind: "none" };

export interface PlanChoice {
  classPackId: string | null;
  isCancellation: boolean;
  question: string;
  consequence: string;
}

export function planStudentStatus(
  student: StudentAppPlanStudent,
  plan: StudentAppShopPlan,
): PlanStudentStatus {
  const hasItNow = student.currentPlanId === plan.id;
  const hasItNextMonth = student.nextMonthPlanId === plan.id;
  if (hasItNow) {
    return hasItNextMonth ? { kind: "active" } : { kind: "ending" };
  }
  if (hasItNextMonth) {
    return { kind: "starting" };
  }
  return student.currentPlanName === null
    ? { kind: "none" }
    : { kind: "other", planName: student.currentPlanName };
}

export function describePlanStudentStatus(status: PlanStudentStatus): string {
  switch (status.kind) {
    case "active":
      return translate("student.shop.planActive");
    case "ending":
      return translate("student.shop.planEnds");
    case "starting":
      return translate("student.shop.planStarts");
    case "other":
      return translate("student.shop.planOther", { plan: status.planName });
    case "none":
      return translate("student.shop.planNone");
  }
}

export function planChoiceFor(
  student: StudentAppPlanStudent,
  plan: StudentAppShopPlan,
  priceLabel: string,
): PlanChoice {
  const status = planStudentStatus(student, plan);
  const names = { plan: plan.name, name: student.fullName };
  switch (status.kind) {
    case "active":
      return {
        classPackId: null,
        isCancellation: true,
        question: translate("student.shop.cancelPlanQuestion", names),
        consequence: translate("student.shop.cancelPlanEnds"),
      };
    case "starting":
      return {
        classPackId: student.currentPlanId,
        isCancellation: true,
        question: translate("student.shop.cancelPlanQuestion", names),
        consequence:
          student.currentPlanName === null
            ? translate("student.shop.cancelPlanNotStarted")
            : translate("student.shop.cancelPlanKeeps", { plan: student.currentPlanName }),
      };
    case "ending":
      return {
        classPackId: plan.id,
        isCancellation: false,
        question: translate("student.shop.choosePlanQuestion", names),
        consequence: translate("student.shop.choosePlanAgain"),
      };
    case "other":
      return {
        classPackId: plan.id,
        isCancellation: false,
        question: translate("student.shop.choosePlanQuestion", names),
        consequence: translate("student.shop.choosePlanNextMonth", { plan: status.planName }),
      };
    case "none":
      return {
        classPackId: plan.id,
        isCancellation: false,
        question: translate("student.shop.choosePlanQuestion", names),
        consequence: translate("student.shop.choosePlanNow", { price: priceLabel }),
      };
  }
}

export function isPlanOfFamily(
  plan: StudentAppShopPlan,
  students: readonly StudentAppPlanStudent[],
): boolean {
  return students.some(
    (student) => student.currentPlanId === plan.id || student.nextMonthPlanId === plan.id,
  );
}

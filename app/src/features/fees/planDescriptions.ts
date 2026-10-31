import { ClassPack } from "@/features/classPacks/types";
import { StudentPlan, StudentPlanChange } from "@/features/fees/types";
import { formatMoney } from "@/formatting/money";
import { translate, translateCount } from "@/i18n/translate";

const includedInTheFamilyFee = 0;

export function monthlyPlanHint(plan: ClassPack, currencyCode: string): string {
  const price = formatMoney(plan.price, currencyCode);
  return plan.classCount === null
    ? translate("fees.studentPlan.planHintLibre", { price })
    : translateCount("fees.studentPlan.planHintClasses", plan.classCount, { price });
}

export function describeStudentPlan(plan: StudentPlan, currencyCode: string): string {
  if (plan.planName === null || plan.price === null) {
    return translate("fees.client.noMonthlyPlan");
  }
  if (plan.customPrice === includedInTheFamilyFee) {
    return translate("fees.client.includedInFamily", { plan: plan.planName });
  }
  const price = formatMoney(plan.price, currencyCode);
  return plan.customPrice === null
    ? translate("fees.client.planLine", { plan: plan.planName, price })
    : translate("fees.client.ownPrice", { plan: plan.planName, price });
}

export function describeStudentPlanChange(change: StudentPlanChange, currencyCode: string): string {
  if (change.planName === null) {
    return translate("fees.client.noMonthlyPlan");
  }
  if (change.customPrice === null) {
    return change.planName;
  }
  return change.customPrice === includedInTheFamilyFee
    ? translate("fees.client.includedInFamily", { plan: change.planName })
    : translate("fees.client.ownPrice", {
        plan: change.planName,
        price: formatMoney(change.customPrice, currencyCode),
      });
}

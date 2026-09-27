import { formatMoney } from "@/features/fees/money";
import { BillingPlan } from "@/features/fees/types";
import { translate } from "@/i18n/translate";

export function describeBillingPlan(
  plan: BillingPlan,
  defaultFee: number | null,
  currencyCode: string,
): string {
  if (plan.kind === "ClassPacks") {
    return translate("fees.client.classPacksPlan");
  }
  if (plan.kind === "CustomFee" && plan.customFee !== null) {
    return translate("fees.client.ownFee", { fee: formatMoney(plan.customFee, currencyCode) });
  }
  return defaultFee === null
    ? translate("fees.client.noFee")
    : translate("fees.client.defaultFee", { fee: formatMoney(defaultFee, currencyCode) });
}

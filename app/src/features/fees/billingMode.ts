import { Business } from "@/features/business/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";

export function hasChosenBillingMode(business: Business): boolean {
  return business.defaultMonthlyFeeChanges.length > 0;
}

export function billingModeValue(business: Business): string {
  if (!hasChosenBillingMode(business)) {
    return translate("settings.monthlyFeeMissing");
  }
  if (business.billingMode === "ClassPacks") {
    return translate("fees.billingMode.ClassPacks");
  }
  return business.defaultMonthlyFee === null
    ? translate("fees.billingMode.MonthlyFee")
    : translate("settings.billingModeMonthlyFee", {
        fee: formatMoney(business.defaultMonthlyFee, business.currencyCode),
      });
}

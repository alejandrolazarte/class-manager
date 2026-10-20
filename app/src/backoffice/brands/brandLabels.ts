import { BrandStatus } from "@/backoffice/brands/brandsApi";
import { hasTranslation, translate, translateCount } from "@/i18n/translate";
import { StatusTone } from "@/ui/StatusPill";

const statusTones: Record<BrandStatus, StatusTone> = {
  Trial: "primary",
  Active: "success",
  Ended: "danger",
  WithoutSubscription: "neutral",
};

export function planLabel(planCode: string | null): string {
  if (planCode === null) {
    return translate("backoffice.brands.status.WithoutSubscription");
  }
  const key = `subscriptions.plan.${planCode}`;
  return hasTranslation(key) ? translate(key) : planCode;
}

export function featureLabel(featureCode: string): string {
  const key = `subscriptions.feature.${featureCode}`;
  return hasTranslation(key) ? translate(key) : featureCode;
}

export function statusLabel(status: BrandStatus, trialDaysLeft: number | null): string {
  return status === "Trial" && trialDaysLeft !== null
    ? translateCount("backoffice.brands.trialDaysLeft", trialDaysLeft)
    : translate(`backoffice.brands.status.${status}`);
}

export function statusTone(status: BrandStatus): StatusTone {
  return statusTones[status];
}

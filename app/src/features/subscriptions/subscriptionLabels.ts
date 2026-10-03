import { hasTranslation, translate } from "@/i18n/translate";

export function planLabel(planCode: string): string {
  const key = `subscriptions.plan.${planCode}`;
  return hasTranslation(key) ? translate(key) : planCode;
}

export function featureLabel(featureCode: string): string {
  const key = `subscriptions.feature.${featureCode}`;
  return hasTranslation(key) ? translate(key) : featureCode;
}

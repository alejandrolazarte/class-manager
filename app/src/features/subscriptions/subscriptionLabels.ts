import { formatMegabytes } from "@/formatting/storageSize";
import { countedFeatureCodes, featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { FeatureLimit } from "@/features/subscriptions/types";
import { hasTranslation, translate } from "@/i18n/translate";

export function planLabel(planCode: string): string {
  const key = `subscriptions.plan.${planCode}`;
  return hasTranslation(key) ? translate(key) : planCode;
}

export function featureLabel(featureCode: string): string {
  const key = `subscriptions.feature.${featureCode}`;
  return hasTranslation(key) ? translate(key) : featureCode;
}

function amountOf(feature: FeatureLimit, amount: number): string | number {
  return feature.code === featureCodes.documentStorage ? formatMegabytes(amount) : amount;
}

export function featureDescription(feature: FeatureLimit): string {
  if (feature.used != null) {
    return feature.limit === null
      ? translate("subscriptions.featureUsedUnlimited", {
          feature: featureLabel(feature.code),
          used: amountOf(feature, feature.used),
        })
      : translate("subscriptions.featureUsed", {
          feature: featureLabel(feature.code),
          used: amountOf(feature, feature.used),
          limit: amountOf(feature, feature.limit),
        });
  }
  if (feature.limit === null) {
    return countedFeatureCodes.includes(feature.code)
      ? translate("subscriptions.featureUnlimited", { feature: featureLabel(feature.code) })
      : featureLabel(feature.code);
  }
  return translate("subscriptions.featureWithLimit", {
    feature: featureLabel(feature.code),
    limit: amountOf(feature, feature.limit),
  });
}

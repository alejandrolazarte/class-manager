import { View } from "react-native";
import {
  countedFeatureCodes,
  featureCodes,
  featureDisplayOrder,
} from "@/features/subscriptions/subscriptionCodes";
import { formatMegabytes } from "@/features/subscriptions/storageSize";
import { featureLabel } from "@/features/subscriptions/subscriptionLabels";
import { FeatureLimit } from "@/features/subscriptions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

function amountOf(feature: FeatureLimit, amount: number): string | number {
  return feature.code === featureCodes.documentStorage ? formatMegabytes(amount) : amount;
}

function featureDescription(feature: FeatureLimit): string {
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

function displayPosition(feature: FeatureLimit): number {
  const position = featureDisplayOrder.indexOf(feature.code);
  return position === -1 ? featureDisplayOrder.length : position;
}

export function FeatureList({ features }: { features: FeatureLimit[] }) {
  return (
    <View className="gap-1.5">
      {features
        .filter((feature) => feature.limit !== 0)
        .sort((first, second) => displayPosition(first) - displayPosition(second))
        .map((feature) => (
          <View key={feature.code} className="flex-row items-center gap-2">
            <Icon name="present" size="small" tone="success" />
            <AppText variant="caption" className="flex-1">
              {featureDescription(feature)}
            </AppText>
          </View>
        ))}
    </View>
  );
}

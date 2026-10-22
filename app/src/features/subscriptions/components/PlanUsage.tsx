import { View } from "react-native";
import { usageOf } from "@/features/subscriptions/planOffer";
import { featureDescription } from "@/features/subscriptions/subscriptionLabels";
import { FeatureCode } from "@/features/subscriptions/subscriptionCodes";
import { useSubscription } from "@/features/subscriptions/useSubscription";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

export function PlanUsage({ featureCode }: { featureCode: FeatureCode }) {
  const usage = usageOf(useSubscription(), featureCode);
  if (usage === undefined || usage.limit === null || usage.used == null) {
    return null;
  }
  const isAtLimit = usage.used >= usage.limit;
  return (
    <View className="gap-1.5">
      <AppText variant="caption" tone={isAtLimit ? "warning" : "muted"}>
        {featureDescription(usage)}
      </AppText>
      <View className="flex-row">
        <ProgressBar
          ratio={usage.limit === 0 ? 1 : usage.used / usage.limit}
          tone={isAtLimit ? "warning" : "primary"}
          isThin
        />
      </View>
    </View>
  );
}

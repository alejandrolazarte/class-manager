import { useRouter } from "expo-router";
import { useEffect } from "react";
import { useOfferPlan } from "@/features/subscriptions/PlanOfferProvider";
import { FeatureCode } from "@/features/subscriptions/subscriptionCodes";
import { routes } from "@/navigation/routes";
import { SkeletonScreen } from "@/ui/SkeletonScreen";

export function PlanOfferInsteadOfScreen({ featureCode }: { featureCode: FeatureCode }) {
  const router = useRouter();
  const offerPlan = useOfferPlan();
  useEffect(() => {
    offerPlan(featureCode);
    if (router.canGoBack()) {
      router.back();
    } else {
      router.replace(routes.settings);
    }
  }, [featureCode, offerPlan, router]);
  return <SkeletonScreen />;
}

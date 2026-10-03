import { useRouter } from "expo-router";
import { planCodes } from "@/features/subscriptions/subscriptionCodes";
import { daysLeftOf, isTrial, useSubscription } from "@/features/subscriptions/useSubscription";
import { translate, translateCount } from "@/i18n/translate";
import { planTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

export function SubscriptionNotice() {
  const router = useRouter();
  const tab = useCurrentTab(planTabs);
  const subscription = useSubscription();
  const seePlans = (
    <Button
      variant="secondary"
      size="medium"
      label={translate("subscriptions.seePlans")}
      onPress={() => router.push(routes.plan(tab))}
    />
  );

  if (!subscription.isActive) {
    return (
      <Banner
        tone="warning"
        icon="locked"
        message={translate(
          subscription.planCode === planCodes.free
            ? "subscriptions.readOnlyTrial"
            : "subscriptions.readOnly",
        )}
      >
        {seePlans}
      </Banner>
    );
  }
  if (isTrial(subscription) && subscription.expiredOn !== null) {
    return (
      <Banner
        tone="info"
        icon="plan"
        message={translateCount("subscriptions.trialDaysLeft", daysLeftOf(subscription.expiredOn))}
      >
        {seePlans}
      </Banner>
    );
  }
  return null;
}

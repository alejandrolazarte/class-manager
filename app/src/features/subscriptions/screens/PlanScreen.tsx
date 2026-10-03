import { formatLongDate } from "@/features/sessions/dates";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { PlanCatalog } from "@/features/subscriptions/components/PlanCatalog";
import { PlansContact } from "@/features/subscriptions/components/PlansContact";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { CurrentSubscription } from "@/features/subscriptions/types";
import {
  daysLeftOf,
  isTrial,
  useOrganizationSubscription,
  useSubscription,
} from "@/features/subscriptions/useSubscription";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";

function statusLabel(subscription: CurrentSubscription): string {
  if (!subscription.isActive) {
    return subscription.endsOn === null
      ? translate("subscriptions.status.ended")
      : translate("subscriptions.status.endedOn", { date: formatLongDate(subscription.endsOn) });
  }
  if (isTrial(subscription) && subscription.endsOn !== null) {
    return translateCount("subscriptions.trialDaysLeft", daysLeftOf(subscription.endsOn));
  }
  if (subscription.endsOn !== null) {
    return translate("subscriptions.status.activeUntil", {
      date: formatLongDate(subscription.endsOn),
    });
  }
  return translate("subscriptions.status.active");
}

export function PlanScreen() {
  const subscription = useSubscription();
  const canViewPrice = useCan(permissions.subscriptionView);
  const organizationSubscription = useOrganizationSubscription({ enabled: canViewPrice });
  const price = organizationSubscription.data;

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("subscriptions.title")} />}
    >
      <Card className="gap-2.5 px-4 py-3.5">
        <AppText variant="eyebrow" tone="accent">
          {translate("subscriptions.yourPlan")}
        </AppText>
        <AppText variant="headline">{planLabel(subscription.planCode)}</AppText>
        <AppText variant="body" tone={subscription.isActive ? "muted" : "danger"}>
          {statusLabel(subscription)}
        </AppText>
        {price && !isTrial(subscription) ? (
          <AppText variant="bodyStrong">
            {translate("subscriptions.pricePerMonth", {
              price: formatMoney(price.price, price.currency),
            })}
          </AppText>
        ) : null}
        <FeatureList features={subscription.features} />
      </Card>
      <PlansContact />
      <SectionTitle title={translate("subscriptions.availablePlans")} />
      <PlanCatalog currentPlanCode={subscription.planCode} />
    </ScrollScreen>
  );
}

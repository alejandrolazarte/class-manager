import { formatLongDate } from "@/dates/dates";
import { formatMoney } from "@/formatting/money";
import { CheckoutReturnNotice } from "@/features/subscriptions/components/CheckoutReturnNotice";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { PastDueNotice } from "@/features/subscriptions/components/PastDueNotice";
import { PlanCatalog } from "@/features/subscriptions/components/PlanCatalog";
import { PlansContact } from "@/features/subscriptions/components/PlansContact";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { CurrentSubscription } from "@/features/subscriptions/types";
import {
  useBillingRedirects,
  useSelfServiceBilling,
} from "@/features/subscriptions/useSelfServiceBilling";
import { daysLeftOf, isTrial, useSubscription } from "@/features/subscriptions/useSubscription";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { TonalBlock } from "@/ui/TonalBlock";

function statusLabel(subscription: CurrentSubscription): string {
  if (!subscription.isActive) {
    return subscription.expiredOn === null
      ? translate("subscriptions.status.ended")
      : translate("subscriptions.status.endedOn", { date: formatLongDate(subscription.expiredOn) });
  }
  if (isTrial(subscription) && subscription.expiredOn !== null) {
    return translateCount("subscriptions.trialDaysLeft", daysLeftOf(subscription.expiredOn));
  }
  if (subscription.expiredOn !== null) {
    return translate("subscriptions.status.activeUntil", {
      date: formatLongDate(subscription.expiredOn),
    });
  }
  return translate("subscriptions.status.active");
}

export function PlanScreen() {
  const subscription = useSubscription();
  const { organizationSubscription, canChoosePlan, canManageBilling, managesOnTheWebOnly } =
    useSelfServiceBilling();
  const { checkout, portal } = useBillingRedirects();
  const price = organizationSubscription.data;

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("subscriptions.title")} />}
    >
      <CheckoutReturnNotice paysOnline={price?.isExternal === true} />
      <PastDueNotice />
      <TonalBlock tone="muted">
        <AppText variant="eyebrow" tone="accent">
          {translate("subscriptions.yourPlan")}
        </AppText>
        <AppText variant="headline">{planLabel(subscription.planCode)}</AppText>
        <AppText variant="body" tone={subscription.isActive ? "muted" : "danger"}>
          {statusLabel(subscription)}
        </AppText>
        {price && !isTrial(subscription) ? (
          <AppText variant="bodyStrong">
            {translate(
              price.isExternal
                ? "subscriptions.pricePerMonthPlusVat"
                : "subscriptions.pricePerMonth",
              { price: formatMoney(price.price, price.currency) },
            )}
          </AppText>
        ) : null}
        <FeatureList features={subscription.features} />
        {canManageBilling ? (
          <Button
            variant="secondary"
            size="medium"
            label={translate("subscriptions.manageBilling")}
            isLoading={portal.isPending}
            onPress={() => portal.mutate()}
          />
        ) : null}
      </TonalBlock>
      {managesOnTheWebOnly ? (
        <AppText variant="body" tone="muted">
          {translate("subscriptions.manageOnWeb")}
        </AppText>
      ) : null}
      <PlansContact />
      <SectionTitle title={translate("subscriptions.availablePlans")} />
      <PlanCatalog
        currentPlanCode={subscription.planCode}
        onChoosePlan={canChoosePlan ? (planCode) => checkout.mutate(planCode) : undefined}
        choosingPlanCode={checkout.isPending ? (checkout.variables ?? null) : null}
      />
    </ScrollScreen>
  );
}

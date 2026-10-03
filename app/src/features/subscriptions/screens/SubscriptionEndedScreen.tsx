import { PlanCatalog } from "@/features/subscriptions/components/PlanCatalog";
import { PlansContact } from "@/features/subscriptions/components/PlansContact";
import { isTrial, useSubscription } from "@/features/subscriptions/useSubscription";
import { planCodes } from "@/features/subscriptions/subscriptionCodes";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function SubscriptionEndedScreen({ onSeeData }: { onSeeData: () => void }) {
  const subscription = useSubscription();
  const wasTrial = isTrial(subscription) || subscription.planCode === planCodes.free;
  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate(
            wasTrial ? "subscriptions.ended.trialTitle" : "subscriptions.ended.title",
          )}
        />
      }
    >
      <AppText variant="lead" tone="muted">
        {translate("subscriptions.ended.body")}
      </AppText>
      <PlansContact />
      <PlanCatalog includesTrial={false} />
      <Button
        variant="secondary"
        icon="search"
        label={translate("subscriptions.ended.seeData")}
        onPress={onSeeData}
      />
    </ScrollScreen>
  );
}

import { formatLongDate } from "@/dates/dates";
import { formatMoney } from "@/formatting/money";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { PlanOffer } from "@/features/subscriptions/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { TonalBlock } from "@/ui/TonalBlock";

interface OfferedPlanProps {
  offer: PlanOffer;
  onAccept?: () => void;
  isAccepting: boolean;
}

export function OfferedPlan({ offer, onAccept, isAccepting }: OfferedPlanProps) {
  return (
    <TonalBlock tone="primary" testID="offered-plan">
      <AppText variant="bodyStrong">
        {translate("subscriptions.offer.title", {
          plan: planLabel(offer.planCode),
          price: formatMoney(offer.price, offer.currency),
        })}
      </AppText>
      {offer.expiredOn !== null ? (
        <AppText variant="body" tone="muted">
          {translate("subscriptions.offer.until", { date: formatLongDate(offer.expiredOn) })}
        </AppText>
      ) : null}
      {onAccept ? (
        <Button
          variant="primary"
          size="medium"
          label={translate("subscriptions.offer.accept")}
          isLoading={isAccepting}
          onPress={onAccept}
        />
      ) : null}
    </TonalBlock>
  );
}

import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import {
  useBillingRedirects,
  useSelfServiceBilling,
} from "@/features/subscriptions/useSelfServiceBilling";

export function PastDueNotice() {
  const { isPastDue, canManageBilling } = useSelfServiceBilling();
  const { portal } = useBillingRedirects();
  if (!isPastDue) {
    return null;
  }
  return (
    <Banner tone="warning" message={translate("subscriptions.pastDue")}>
      {canManageBilling ? (
        <Button
          variant="secondary"
          size="medium"
          label={translate("subscriptions.updateCard")}
          isLoading={portal.isPending}
          onPress={() => portal.mutate()}
        />
      ) : null}
    </Banner>
  );
}

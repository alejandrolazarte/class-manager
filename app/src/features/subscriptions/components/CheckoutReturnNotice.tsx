import { useQueryClient } from "@tanstack/react-query";
import { useLocalSearchParams } from "expo-router";
import { useEffect, useState } from "react";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { subscriptionQueryKeys } from "@/features/subscriptions/subscriptionQueryKeys";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";

export const checkoutSucceeded = "success";
export const checkoutPollingIntervalInMilliseconds = 3000;
export const checkoutPollingAttempts = 20;

export function CheckoutReturnNotice({ paysOnline }: { paysOnline: boolean }) {
  const { checkout } = useLocalSearchParams<{ checkout?: string }>();
  const queryClient = useQueryClient();
  const [attempts, setAttempts] = useState(0);
  const isWaiting =
    checkout === checkoutSucceeded && !paysOnline && attempts < checkoutPollingAttempts;

  useEffect(() => {
    if (!isWaiting) {
      return undefined;
    }
    const timer = setTimeout(() => {
      void queryClient.invalidateQueries({ queryKey: memberQueryKeys.current() });
      void queryClient.invalidateQueries({
        queryKey: subscriptionQueryKeys.organizationSubscription(),
      });
      setAttempts((previousAttempts) => previousAttempts + 1);
    }, checkoutPollingIntervalInMilliseconds);
    return () => clearTimeout(timer);
  }, [isWaiting, attempts, queryClient]);

  if (checkout !== checkoutSucceeded) {
    return null;
  }
  if (paysOnline) {
    return <Banner tone="success" message={translate("subscriptions.checkoutDone")} />;
  }
  if (isWaiting) {
    return <Banner tone="info" message={translate("subscriptions.checkoutProcessing")} />;
  }
  return <Banner tone="warning" message={translate("subscriptions.checkoutSlow")} />;
}

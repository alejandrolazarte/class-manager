import { PropsWithChildren, useState } from "react";
import { SubscriptionEndedScreen } from "@/features/subscriptions/screens/SubscriptionEndedScreen";
import { useSubscription } from "@/features/subscriptions/useSubscription";

export function SubscriptionGate({ children }: PropsWithChildren) {
  const subscription = useSubscription();
  const [isBrowsingReadOnly, setIsBrowsingReadOnly] = useState(false);
  if (!subscription.isActive && !isBrowsingReadOnly) {
    return <SubscriptionEndedScreen onSeeData={() => setIsBrowsingReadOnly(true)} />;
  }
  return children;
}

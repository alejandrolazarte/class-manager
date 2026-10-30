import { useMutation } from "@tanstack/react-query";
import { Platform } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { goToBillingPage } from "@/features/subscriptions/billingRedirect";
import { openBillingPortal, startPlanCheckout } from "@/features/subscriptions/subscriptionsApi";
import { BillingRedirect } from "@/features/subscriptions/types";
import { useOrganizationSubscription } from "@/features/subscriptions/useSubscription";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

const pastDueStatus = "PastDue";

export function useSelfServiceBilling() {
  const canViewSubscription = useCan(permissions.subscriptionView);
  const canManageSubscription = useCan(permissions.subscriptionManage);
  const organizationSubscription = useOrganizationSubscription({ enabled: canViewSubscription });
  const subscription = organizationSubscription.data;
  const isWeb = Platform.OS === "web";
  const paysOnline = subscription?.isExternal === true;
  const mayPayOnline = canManageSubscription && (subscription?.selfService === true || paysOnline);
  const offer = canManageSubscription && !paysOnline ? (subscription?.offer ?? null) : null;

  return {
    organizationSubscription,
    canChoosePlan: isWeb && mayPayOnline && !paysOnline,
    canManageBilling: isWeb && canManageSubscription && paysOnline,
    offer,
    canAcceptOffer: isWeb && offer !== null,
    managesOnTheWebOnly: !isWeb && (mayPayOnline || offer !== null),
    isPastDue: canManageSubscription && subscription?.paymentStatus === pastDueStatus,
  };
}

export function useBillingRedirects() {
  const { showToast } = useToast();
  const redirectOptions = {
    onSuccess: (redirect: BillingRedirect) => goToBillingPage(redirect.url),
    onError: () => showToast(translate("common.unexpectedError")),
  };
  const checkout = useMutation({ mutationFn: startPlanCheckout, ...redirectOptions });
  const portal = useMutation({ mutationFn: openBillingPortal, ...redirectOptions });

  return { checkout, portal };
}

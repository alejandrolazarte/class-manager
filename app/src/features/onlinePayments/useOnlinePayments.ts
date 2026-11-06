import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { onlinePaymentQueryKeys } from "@/features/onlinePayments/onlinePaymentQueryKeys";
import {
  getOnlinePayments,
  refreshOnlinePaymentAccount,
  startOnlinePaymentOnboarding,
  stopUsingOnlinePaymentAccount,
  chooseOnlinePaymentAccount,
} from "@/features/onlinePayments/onlinePaymentsApi";
import { goToStripePage } from "@/features/onlinePayments/stripeRedirect";
import { OnlinePayments } from "@/features/onlinePayments/types";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

export function useOnlinePayments() {
  const canManageOnlinePayments = useCan(permissions.onlinePaymentsManage);
  return useQuery({
    queryKey: onlinePaymentQueryKeys.all,
    queryFn: getOnlinePayments,
    enabled: canManageOnlinePayments,
  });
}

export function useOnlinePaymentActions() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const showError = () => showToast(translate("common.unexpectedError"));
  const storeOnlinePayments = (onlinePayments: OnlinePayments) =>
    queryClient.setQueryData(onlinePaymentQueryKeys.all, onlinePayments);

  const onboarding = useMutation({
    mutationFn: startOnlinePaymentOnboarding,
    onSuccess: (redirect) => goToStripePage(redirect.url),
    onError: showError,
  });
  const refresh = useMutation({
    mutationFn: refreshOnlinePaymentAccount,
    onSuccess: storeOnlinePayments,
    onError: showError,
  });
  const chooseAccount = useMutation({
    mutationFn: chooseOnlinePaymentAccount,
    onSuccess: storeOnlinePayments,
    onError: showError,
  });
  const stopUsing = useMutation({
    mutationFn: stopUsingOnlinePaymentAccount,
    onSuccess: storeOnlinePayments,
    onError: showError,
  });

  return { onboarding, refresh, chooseAccount, stopUsing };
}

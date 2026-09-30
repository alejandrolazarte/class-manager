import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getPushKey,
  removePushSubscription,
  savePushSubscription,
} from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import {
  browserPushSupport,
  BrowserPushSupport,
  currentBrowserSubscription,
  subscribeBrowser,
  unsubscribeBrowser,
} from "@/features/family/push/browserPush";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

export type FamilyNotificationsStatus = BrowserPushSupport | "unavailable";

export async function stopFamilyNotifications(): Promise<void> {
  const endpoint = await unsubscribeBrowser();
  if (endpoint !== null) {
    await removePushSubscription(endpoint);
  }
}

export function useFamilyNotifications() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const support = browserPushSupport();
  const { data: pushKey } = useQuery({
    queryKey: familyQueryKeys.pushKey(),
    queryFn: getPushKey,
    enabled: support !== "unsupported",
  });
  const { data: subscription } = useQuery({
    queryKey: familyQueryKeys.pushSubscription(),
    queryFn: currentBrowserSubscription,
    enabled: support === "supported",
  });
  const mutation = useMutation({
    mutationFn: async (turnOn: boolean) => {
      if (!turnOn) {
        await stopFamilyNotifications();
        return false;
      }
      const created = await subscribeBrowser(pushKey?.publicKey ?? "");
      if (created === null) {
        return false;
      }
      await savePushSubscription(created);
      return true;
    },
    onSuccess: (isOn, turnOn) => {
      if (turnOn && !isOn) {
        showToast(translate("family.notifications.notAllowed"));
      }
      return queryClient.invalidateQueries({ queryKey: familyQueryKeys.pushSubscription() });
    },
    onError: () => showToast(translate("common.unexpectedError")),
  });

  const status: FamilyNotificationsStatus =
    support === "supported" && !pushKey?.publicKey ? "unavailable" : support;
  return {
    status,
    isOn: subscription != null,
    isPending: mutation.isPending,
    toggle: (turnOn: boolean) => mutation.mutate(turnOn),
  };
}

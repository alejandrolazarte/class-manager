import { QueryKey, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  browserPushSupport,
  BrowserPushSubscription,
  BrowserPushSupport,
  currentBrowserSubscription,
  subscribeBrowser,
  unsubscribeBrowser,
} from "@/features/studentApp/push/browserPush";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

export type PushNotificationsStatus = BrowserPushSupport | "unavailable";

export interface PushChannel {
  pushKeyQueryKey: QueryKey;
  subscriptionQueryKey: QueryKey;
  getPushKey: () => Promise<{ publicKey: string | null }>;
  saveSubscription: (subscription: BrowserPushSubscription) => Promise<void>;
  removeSubscription: (endpoint: string) => Promise<void>;
}

export interface PushNotifications {
  status: PushNotificationsStatus;
  isOn: boolean;
  isPending: boolean;
  toggle: (turnOn: boolean) => void;
}

export async function stopPushNotifications(channel: PushChannel): Promise<void> {
  const endpoint = await unsubscribeBrowser();
  if (endpoint !== null) {
    await channel.removeSubscription(endpoint);
  }
}

export function usePushNotifications(channel: PushChannel): PushNotifications {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const support = browserPushSupport();
  const { data: pushKey } = useQuery({
    queryKey: channel.pushKeyQueryKey,
    queryFn: channel.getPushKey,
    enabled: support !== "unsupported",
  });
  const { data: subscription } = useQuery({
    queryKey: channel.subscriptionQueryKey,
    queryFn: currentBrowserSubscription,
    enabled: support === "supported",
  });
  const mutation = useMutation({
    mutationFn: async (turnOn: boolean) => {
      if (!turnOn) {
        await stopPushNotifications(channel);
        return false;
      }
      const created = await subscribeBrowser(pushKey?.publicKey ?? "");
      if (created === null) {
        return false;
      }
      await channel.saveSubscription(created);
      return true;
    },
    onSuccess: (isOn, turnOn) => {
      if (turnOn && !isOn) {
        showToast(translate("student.notifications.notAllowed"));
      }
      return queryClient.invalidateQueries({ queryKey: channel.subscriptionQueryKey });
    },
    onError: () => showToast(translate("common.unexpectedError")),
  });

  return {
    status: support === "supported" && !pushKey?.publicKey ? "unavailable" : support,
    isOn: subscription != null,
    isPending: mutation.isPending,
    toggle: (turnOn: boolean) => mutation.mutate(turnOn),
  };
}

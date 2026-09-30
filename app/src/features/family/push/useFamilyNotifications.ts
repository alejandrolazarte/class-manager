import {
  getPushKey,
  removePushSubscription,
  savePushSubscription,
} from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import {
  PushChannel,
  stopPushNotifications,
  usePushNotifications,
} from "@/features/notifications/usePushNotifications";

const familyPushChannel: PushChannel = {
  pushKeyQueryKey: familyQueryKeys.pushKey(),
  subscriptionQueryKey: familyQueryKeys.pushSubscription(),
  getPushKey: () => getPushKey(),
  saveSubscription: (subscription) => savePushSubscription(subscription),
  removeSubscription: (endpoint) => removePushSubscription(endpoint),
};

export function stopFamilyNotifications(): Promise<void> {
  return stopPushNotifications(familyPushChannel);
}

export function useFamilyNotifications() {
  return usePushNotifications(familyPushChannel);
}

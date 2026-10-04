import {
  getPushKey,
  removePushSubscription,
  savePushSubscription,
} from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import {
  PushChannel,
  stopPushNotifications,
  usePushNotifications,
} from "@/features/notifications/usePushNotifications";

const studentAppPushChannel: PushChannel = {
  pushKeyQueryKey: studentAppQueryKeys.pushKey(),
  subscriptionQueryKey: studentAppQueryKeys.pushSubscription(),
  getPushKey: () => getPushKey(),
  saveSubscription: (subscription) => savePushSubscription(subscription),
  removeSubscription: (endpoint) => removePushSubscription(endpoint),
};

export function stopStudentAppNotifications(): Promise<void> {
  return stopPushNotifications(studentAppPushChannel);
}

export function useStudentAppNotifications() {
  return usePushNotifications(studentAppPushChannel);
}

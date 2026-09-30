import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  PushChannel,
  stopPushNotifications,
  usePushNotifications,
} from "@/features/notifications/usePushNotifications";
import { teamNotificationQueryKeys } from "@/features/teamNotifications/teamNotificationQueryKeys";
import {
  getTeamNotifications,
  getTeamPushKey,
  markTeamNotificationsSeen,
  removeTeamPushSubscription,
  saveTeamPushSubscription,
} from "@/features/teamNotifications/teamNotificationsApi";

const teamPushChannel: PushChannel = {
  pushKeyQueryKey: teamNotificationQueryKeys.pushKey(),
  subscriptionQueryKey: teamNotificationQueryKeys.pushSubscription(),
  getPushKey: () => getTeamPushKey(),
  saveSubscription: (subscription) => saveTeamPushSubscription(subscription),
  removeSubscription: (endpoint) => removeTeamPushSubscription(endpoint),
};

export function useTeamNotifications() {
  return useQuery({ queryKey: teamNotificationQueryKeys.list(), queryFn: getTeamNotifications });
}

export function useMarkTeamNotificationsSeen() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => markTeamNotificationsSeen(),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: teamNotificationQueryKeys.list() }),
  });
}

export function useTeamPushNotifications() {
  return usePushNotifications(teamPushChannel);
}

export function stopTeamPushNotifications(): Promise<void> {
  return stopPushNotifications(teamPushChannel);
}

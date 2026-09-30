import { httpClient } from "@/api/httpClient";
import { BrowserPushSubscription } from "@/features/family/push/browserPush";
import { TeamNotifications, TeamPushKey } from "@/features/teamNotifications/types";

const teamPath = "/api/team";
const notificationsPath = `${teamPath}/notifications`;
const seenPath = `${notificationsPath}/seen`;
const pushKeyPath = `${teamPath}/push-key`;
const pushSubscriptionPath = `${teamPath}/push-subscription`;

export function getTeamNotifications(): Promise<TeamNotifications> {
  return httpClient.get<TeamNotifications>(notificationsPath);
}

export function markTeamNotificationsSeen(): Promise<void> {
  return httpClient.put<void>(seenPath, {});
}

export function getTeamPushKey(): Promise<TeamPushKey> {
  return httpClient.get<TeamPushKey>(pushKeyPath);
}

export function saveTeamPushSubscription(subscription: BrowserPushSubscription): Promise<void> {
  return httpClient.put<void>(pushSubscriptionPath, subscription);
}

export function removeTeamPushSubscription(endpoint: string): Promise<void> {
  return httpClient.delete<void>(
    `${pushSubscriptionPath}?endpoint=${encodeURIComponent(endpoint)}`,
  );
}

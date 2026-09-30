export const teamNotificationQueryKeys = {
  all: ["team-notifications"] as const,
  list: () => [...teamNotificationQueryKeys.all, "list"] as const,
  pushKey: () => [...teamNotificationQueryKeys.all, "push-key"] as const,
  pushSubscription: () => [...teamNotificationQueryKeys.all, "push-subscription"] as const,
};

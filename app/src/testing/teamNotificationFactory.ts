import { TeamNotification, TeamNotifications } from "@/features/teamNotifications/types";

export function buildTeamNotification(overrides: Partial<TeamNotification> = {}): TeamNotification {
  return {
    id: "notification-1",
    title: "Tomás avisó que no viene",
    body: "Natación inicial · mié 1/10 18:00",
    url: "/today/class-group-1/2026-10-01",
    createdAt: new Date().toISOString(),
    isUnread: true,
    ...overrides,
  };
}

export function buildTeamNotifications(
  overrides: Partial<TeamNotifications> = {},
): TeamNotifications {
  return { items: [buildTeamNotification()], unreadCount: 1, ...overrides };
}

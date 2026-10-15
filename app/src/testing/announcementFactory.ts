import { Announcement } from "@/features/announcements/types";

export function buildAnnouncement(overrides: Partial<Announcement> = {}): Announcement {
  return {
    id: "announcement-1",
    title: "Lunes 12 cerrado",
    body: null,
    publishedAt: "2026-09-30T10:00:00Z",
    isScheduled: false,
    category: "News",
    isPinned: false,
    visibleUntil: null,
    classGroups: [],
    images: [],
    recipientCount: 0,
    seenCount: 0,
    goingCount: 0,
    notGoingCount: 0,
    action: "None",
    actionUrl: null,
    ...overrides,
  };
}

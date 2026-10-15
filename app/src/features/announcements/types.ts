import { CatalogImage } from "@/features/catalogImages/types";

export type AnnouncementCategory = "News" | "Important" | "Event" | "Reminder";

export interface AnnouncementClassGroup {
  id: string;
  name: string;
}

export interface Announcement {
  id: string;
  title: string;
  body: string | null;
  publishedAt: string;
  isScheduled: boolean;
  category: AnnouncementCategory;
  isPinned: boolean;
  visibleUntil: string | null;
  classGroups: AnnouncementClassGroup[];
  images: CatalogImage[];
  recipientCount: number;
  seenCount: number;
  goingCount: number;
  notGoingCount: number;
}

export interface AnnouncementRecipient {
  clientId: string;
  fullName: string;
  seenAt: string | null;
  isGoing: boolean | null;
}

export interface AnnouncementReminder {
  remindedCount: number;
}

export interface AnnouncementRequest {
  title: string;
  body: string | null;
  category: AnnouncementCategory;
  classGroupIds: string[];
  isPinned: boolean;
  visibleUntil: string | null;
  publishOn: string | null;
}

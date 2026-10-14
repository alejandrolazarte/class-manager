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
  category: AnnouncementCategory;
  isPinned: boolean;
  visibleUntil: string | null;
  classGroups: AnnouncementClassGroup[];
  images: CatalogImage[];
}

export interface CreateAnnouncementRequest {
  title: string;
  body: string | null;
  category: AnnouncementCategory;
  classGroupIds: string[];
  isPinned: boolean;
  visibleUntil: string | null;
}

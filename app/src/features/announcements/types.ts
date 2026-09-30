export interface Announcement {
  id: string;
  title: string;
  body: string | null;
  publishedAt: string;
}

export interface CreateAnnouncementRequest {
  title: string;
  body: string | null;
}

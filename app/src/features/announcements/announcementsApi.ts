import { httpClient } from "@/api/httpClient";
import { Announcement, CreateAnnouncementRequest } from "@/features/announcements/types";

const announcementsPath = "/api/announcements";
const restoreSegment = "restore";

export function listAnnouncements(): Promise<Announcement[]> {
  return httpClient.get<Announcement[]>(announcementsPath);
}

export function createAnnouncement(request: CreateAnnouncementRequest): Promise<Announcement> {
  return httpClient.post<Announcement>(announcementsPath, request);
}

export function deleteAnnouncement(announcementId: string): Promise<void> {
  return httpClient.delete<void>(`${announcementsPath}/${encodeURIComponent(announcementId)}`);
}

export function restoreAnnouncement(announcementId: string): Promise<void> {
  return httpClient.post<void>(
    `${announcementsPath}/${encodeURIComponent(announcementId)}/${restoreSegment}`,
    {},
  );
}

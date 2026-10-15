import { PickedFile, toFileForm } from "@/api/fileForm";
import { httpClient } from "@/api/httpClient";
import {
  Announcement,
  AnnouncementRecipient,
  AnnouncementReminder,
  AnnouncementRequest,
} from "@/features/announcements/types";

const announcementsPath = "/api/announcements";
const restoreSegment = "restore";
const pinSegment = "pin";
const imagesSegment = "images";
const recipientsSegment = "recipients";
const reminderSegment = "reminder";

function announcementPath(announcementId: string): string {
  return `${announcementsPath}/${encodeURIComponent(announcementId)}`;
}

export function listAnnouncements(): Promise<Announcement[]> {
  return httpClient.get<Announcement[]>(announcementsPath);
}

export function createAnnouncement(request: AnnouncementRequest): Promise<Announcement> {
  return httpClient.post<Announcement>(announcementsPath, request);
}

export function updateAnnouncement(
  announcementId: string,
  request: AnnouncementRequest,
): Promise<Announcement> {
  return httpClient.put<Announcement>(announcementPath(announcementId), request);
}

export function deleteAnnouncement(announcementId: string): Promise<void> {
  return httpClient.delete<void>(announcementPath(announcementId));
}

export function restoreAnnouncement(announcementId: string): Promise<void> {
  return httpClient.post<void>(`${announcementPath(announcementId)}/${restoreSegment}`, {});
}

export function pinAnnouncement(announcementId: string): Promise<void> {
  return httpClient.post<void>(`${announcementPath(announcementId)}/${pinSegment}`, {});
}

export function unpinAnnouncement(announcementId: string): Promise<void> {
  return httpClient.delete<void>(`${announcementPath(announcementId)}/${pinSegment}`);
}

export function addAnnouncementImage(
  announcementId: string,
  file: PickedFile,
): Promise<Announcement> {
  return httpClient.postForm<Announcement>(
    `${announcementPath(announcementId)}/${imagesSegment}`,
    toFileForm(file),
  );
}

export function removeAnnouncementImage(
  announcementId: string,
  imageId: string,
): Promise<Announcement> {
  return httpClient.delete<Announcement>(
    `${announcementPath(announcementId)}/${imagesSegment}/${encodeURIComponent(imageId)}`,
  );
}

export function listAnnouncementRecipients(
  announcementId: string,
): Promise<AnnouncementRecipient[]> {
  return httpClient.get<AnnouncementRecipient[]>(
    `${announcementPath(announcementId)}/${recipientsSegment}`,
  );
}

export function remindAnnouncement(announcementId: string): Promise<AnnouncementReminder> {
  return httpClient.post<AnnouncementReminder>(
    `${announcementPath(announcementId)}/${reminderSegment}`,
    {},
  );
}

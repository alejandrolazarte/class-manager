import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  addAnnouncementImage,
  createAnnouncement,
  deleteAnnouncement,
  listAnnouncementRecipients,
  listAnnouncements,
  pinAnnouncement,
  remindAnnouncement,
  removeAnnouncementImage,
  restoreAnnouncement,
  unpinAnnouncement,
  updateAnnouncement,
} from "@/features/announcements/announcementsApi";
import { AnnouncementRequest } from "@/features/announcements/types";
import {
  applyCatalogImageDraft,
  CatalogImageDraft,
} from "@/features/catalogImages/catalogImageDraft";
import { CatalogImage } from "@/features/catalogImages/types";

const announcementsQueryKey = ["announcements"] as const;

export function useAnnouncements() {
  return useQuery({ queryKey: announcementsQueryKey, queryFn: listAnnouncements });
}

function useInvalidateAnnouncements() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: announcementsQueryKey });
}

export function useCreateAnnouncement() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: (request: AnnouncementRequest) => createAnnouncement(request),
    onSuccess: invalidate,
  });
}

export function useUpdateAnnouncement() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: ({
      announcementId,
      request,
    }: {
      announcementId: string;
      request: AnnouncementRequest;
    }) => updateAnnouncement(announcementId, request),
    onSuccess: invalidate,
  });
}

interface ApplyAnnouncementImagesInput {
  announcementId: string;
  draft: CatalogImageDraft;
  savedImages: CatalogImage[];
}

export function useApplyAnnouncementImages() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: ({ announcementId, draft, savedImages }: ApplyAnnouncementImagesInput) =>
      applyCatalogImageDraft(draft, announcementId, savedImages, {
        add: addAnnouncementImage,
        remove: removeAnnouncementImage,
      }),
    onSettled: invalidate,
  });
}

export function useDeleteAnnouncement() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: (announcementId: string) => deleteAnnouncement(announcementId),
    onSuccess: invalidate,
  });
}

export function useRestoreAnnouncement() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: (announcementId: string) => restoreAnnouncement(announcementId),
    onSuccess: invalidate,
  });
}

export function useSetAnnouncementPinned() {
  const invalidate = useInvalidateAnnouncements();
  return useMutation({
    mutationFn: ({ announcementId, isPinned }: { announcementId: string; isPinned: boolean }) =>
      isPinned ? pinAnnouncement(announcementId) : unpinAnnouncement(announcementId),
    onSuccess: invalidate,
  });
}

export function useAnnouncementRecipients(announcementId: string) {
  return useQuery({
    queryKey: [...announcementsQueryKey, announcementId, "recipients"],
    queryFn: () => listAnnouncementRecipients(announcementId),
  });
}

export function useRemindAnnouncement() {
  return useMutation({
    mutationFn: (announcementId: string) => remindAnnouncement(announcementId),
  });
}

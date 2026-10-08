import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createAnnouncement,
  deleteAnnouncement,
  listAnnouncements,
  restoreAnnouncement,
} from "@/features/announcements/announcementsApi";
import { CreateAnnouncementRequest } from "@/features/announcements/types";

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
    mutationFn: (request: CreateAnnouncementRequest) => createAnnouncement(request),
    onSuccess: invalidate,
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

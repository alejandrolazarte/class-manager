import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getStudentAppNews,
  markStudentAppNewsSeen,
  replyToAnnouncement,
} from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { StudentAppNews } from "@/features/studentApp/types";

export function useStudentAppNews() {
  return useQuery({ queryKey: studentAppQueryKeys.news(), queryFn: getStudentAppNews });
}

export function useMarkStudentAppNewsSeen() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: markStudentAppNewsSeen,
    onSuccess: () =>
      queryClient.setQueryData<StudentAppNews>(
        studentAppQueryKeys.news(),
        (news) => news && { ...news, unreadCount: 0 },
      ),
  });
}

export function useReplyToAnnouncement() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ announcementId, isGoing }: { announcementId: string; isGoing: boolean }) =>
      replyToAnnouncement(announcementId, isGoing),
    onSuccess: (_, { announcementId, isGoing }) =>
      queryClient.setQueryData<StudentAppNews>(
        studentAppQueryKeys.news(),
        (news) =>
          news && {
            ...news,
            items: news.items.map((item) =>
              item.announcementId === announcementId ? { ...item, isGoing } : item,
            ),
          },
      ),
  });
}

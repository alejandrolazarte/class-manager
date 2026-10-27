import { useQuery } from "@tanstack/react-query";
import { privateLessonQueryKeys } from "@/features/privateLessons/privateLessonQueryKeys";
import { listInstructorAgenda } from "@/features/privateLessons/privateLessonsApi";

export function useInstructorAgenda(
  instructorId: string,
  date: string | null,
  ignoredPrivateLessonId: string | undefined,
) {
  return useQuery({
    queryKey: privateLessonQueryKeys.instructorAgenda(
      instructorId,
      date ?? "",
      ignoredPrivateLessonId ?? "",
    ),
    queryFn: () => listInstructorAgenda(instructorId, date ?? "", ignoredPrivateLessonId),
    enabled: instructorId.length > 0 && date !== null,
  });
}

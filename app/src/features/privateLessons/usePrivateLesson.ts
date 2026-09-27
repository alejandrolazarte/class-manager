import { useQuery } from "@tanstack/react-query";
import { privateLessonQueryKeys } from "@/features/privateLessons/privateLessonQueryKeys";
import { getPrivateLesson } from "@/features/privateLessons/privateLessonsApi";

export function usePrivateLesson(privateLessonId: string | undefined) {
  return useQuery({
    queryKey: privateLessonQueryKeys.lesson(privateLessonId ?? ""),
    queryFn: () => getPrivateLesson(privateLessonId ?? ""),
    enabled: privateLessonId !== undefined,
  });
}

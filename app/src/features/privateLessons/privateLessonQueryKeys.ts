export const privateLessonQueryKeys = {
  all: ["privateLessons"] as const,
  lesson: (privateLessonId: string) => [...privateLessonQueryKeys.all, privateLessonId] as const,
};

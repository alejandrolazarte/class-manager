export const privateLessonQueryKeys = {
  all: ["privateLessons"] as const,
  lesson: (privateLessonId: string) => [...privateLessonQueryKeys.all, privateLessonId] as const,
  instructorAgenda: (instructorId: string, date: string, ignoredPrivateLessonId: string) =>
    [
      ...privateLessonQueryKeys.all,
      "instructorAgenda",
      instructorId,
      date,
      ignoredPrivateLessonId,
    ] as const,
};

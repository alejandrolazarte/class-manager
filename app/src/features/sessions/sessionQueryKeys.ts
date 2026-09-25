export const sessionQueryKeys = {
  all: ["sessions"] as const,
  day: (sessionDate: string) => [...sessionQueryKeys.all, "day", sessionDate] as const,
  session: (classGroupId: string, sessionDate: string) =>
    [...sessionQueryKeys.all, "session", classGroupId, sessionDate] as const,
};

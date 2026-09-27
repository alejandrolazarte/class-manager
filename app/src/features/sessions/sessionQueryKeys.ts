export const sessionQueryKeys = {
  all: ["sessions"] as const,
  day: (sessionDate: string) => [...sessionQueryKeys.all, "day", sessionDate] as const,
  calendars: () => [...sessionQueryKeys.all, "calendar"] as const,
  calendar: (month: string) => [...sessionQueryKeys.calendars(), month] as const,
  session: (classGroupId: string, sessionDate: string) =>
    [...sessionQueryKeys.all, "session", classGroupId, sessionDate] as const,
};

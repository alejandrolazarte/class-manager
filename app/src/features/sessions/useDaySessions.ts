import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import { listDaySessions } from "@/features/sessions/sessionsApi";

export function useDaySessions(sessionDate: string) {
  return useQuery({
    queryKey: sessionQueryKeys.day(sessionDate),
    queryFn: () => listDaySessions(sessionDate),
    placeholderData: keepPreviousData,
  });
}

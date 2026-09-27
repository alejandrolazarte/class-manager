import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import { getMonthCalendar } from "@/features/sessions/sessionsApi";

export function useMonthCalendar(month: string, isEnabled: boolean) {
  return useQuery({
    queryKey: sessionQueryKeys.calendar(month),
    queryFn: () => getMonthCalendar(month),
    placeholderData: keepPreviousData,
    enabled: isEnabled,
  });
}

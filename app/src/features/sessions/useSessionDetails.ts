import { useQuery } from "@tanstack/react-query";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import { getSession } from "@/features/sessions/sessionsApi";

export function useSessionDetails(classGroupId: string, sessionDate: string) {
  return useQuery({
    queryKey: sessionQueryKeys.session(classGroupId, sessionDate),
    queryFn: () => getSession(classGroupId, sessionDate),
  });
}

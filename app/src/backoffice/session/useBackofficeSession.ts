import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback } from "react";
import { rememberAccessToken } from "@/backoffice/api/backofficeHttpClient";
import {
  CurrentOperator,
  getCurrentOperator,
  refreshBackofficeSession,
  signOutOfBackoffice,
} from "@/backoffice/session/backofficeSessionApi";

const sessionQueryKey = ["backofficeSession"];

async function restoreSession(): Promise<CurrentOperator | null> {
  const accessToken = await refreshBackofficeSession();
  rememberAccessToken(accessToken);
  return accessToken === null ? null : getCurrentOperator(accessToken);
}

export function useBackofficeSession() {
  const queryClient = useQueryClient();
  const session = useQuery({ queryKey: sessionQueryKey, queryFn: restoreSession, retry: false });

  const signOut = useCallback(async () => {
    await signOutOfBackoffice().catch(() => undefined);
    rememberAccessToken(null);
    queryClient.setQueryData(sessionQueryKey, null);
  }, [queryClient]);

  return { session, signOut };
}

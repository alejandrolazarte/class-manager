import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback } from "react";
import { forgetAccessToken, restoreAccessToken } from "@/backoffice/api/backofficeHttpClient";
import {
  CurrentOperator,
  getCurrentOperator,
  signOutOfBackoffice,
} from "@/backoffice/session/backofficeSessionApi";

const sessionQueryKey = ["backofficeSession"];
const platformOperatorRole = "PlatformOperator";

async function restoreSession(): Promise<CurrentOperator | null> {
  const accessToken = await restoreAccessToken();
  return accessToken === null ? null : getCurrentOperator(accessToken);
}

export function useBackofficeSession() {
  const queryClient = useQueryClient();
  const session = useQuery({ queryKey: sessionQueryKey, queryFn: restoreSession, retry: false });

  const signOut = useCallback(async () => {
    await signOutOfBackoffice().catch(() => undefined);
    forgetAccessToken();
    queryClient.setQueryData(sessionQueryKey, null);
  }, [queryClient]);

  return { session, signOut, canManage: session.data?.role === platformOperatorRole };
}

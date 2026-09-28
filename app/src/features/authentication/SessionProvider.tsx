import { useQueryClient } from "@tanstack/react-query";
import { createContext, PropsWithChildren, useCallback, useEffect, useMemo, useState } from "react";
import { authenticationSession } from "@/api/authenticationSession";
import {
  acceptInvitation as acceptInvitationRequest,
  refreshSession,
  signIn as signInRequest,
  signOut as signOutRequest,
  signUp as signUpRequest,
  switchBranch as switchBranchRequest,
} from "@/features/authentication/authenticationApi";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import {
  AcceptInvitationRequest,
  SignInRequest,
  SignUpRequest,
  TokenResponse,
} from "@/features/authentication/types";

export type SessionStartReason = "signUp" | "signIn" | "invitation" | "branchSwitch" | "restore";

export type SessionState =
  | { status: "restoring" }
  | { status: "signedOut" }
  | { status: "signedIn"; startedBy: SessionStartReason; ownerFullName?: string };

export interface SessionContextValue {
  session: SessionState;
  signIn: (request: SignInRequest) => Promise<void>;
  signUp: (request: SignUpRequest) => Promise<void>;
  acceptInvitation: (request: AcceptInvitationRequest) => Promise<void>;
  switchBranch: (businessId: string) => Promise<void>;
  signOut: () => Promise<void>;
}

class MissingRefreshTokenError extends Error {
  constructor() {
    super("No refresh token is stored");
    this.name = "MissingRefreshTokenError";
  }
}

const signedOutSession: SessionState = { status: "signedOut" };

export const SessionContext = createContext<SessionContextValue | null>(null);

async function refreshAccessToken(): Promise<string> {
  const storedRefreshToken = await refreshTokenStorage.read();
  if (storedRefreshToken === null) {
    throw new MissingRefreshTokenError();
  }
  const rotatedTokens = await refreshSession({ refreshToken: storedRefreshToken });
  await refreshTokenStorage.write(rotatedTokens.refreshToken);
  return rotatedTokens.accessToken;
}

async function restoreSession(): Promise<SessionState> {
  const storedRefreshToken = await refreshTokenStorage.read();
  if (storedRefreshToken === null) {
    return signedOutSession;
  }
  try {
    const restoredAccessToken = await authenticationSession.refreshAccessToken();
    return restoredAccessToken === null
      ? signedOutSession
      : { status: "signedIn", startedBy: "restore" };
  } catch {
    return signedOutSession;
  }
}

export function SessionProvider({ children }: PropsWithChildren) {
  const queryClient = useQueryClient();
  const [session, setSession] = useState<SessionState>({ status: "restoring" });

  useEffect(() => {
    let isMounted = true;
    authenticationSession.configure({
      refreshAccessToken,
      onSessionEnded: () => {
        refreshTokenStorage.clear().catch(() => undefined);
        queryClient.clear();
        if (isMounted) {
          setSession(signedOutSession);
        }
      },
    });
    restoreSession().then((restoredSession) => {
      if (isMounted) {
        setSession(restoredSession);
      }
    });
    return () => {
      isMounted = false;
    };
  }, [queryClient]);

  const startSession = useCallback(
    async (tokens: TokenResponse, startedBy: SessionStartReason, ownerFullName?: string) => {
      authenticationSession.startSession(tokens.accessToken);
      await refreshTokenStorage.write(tokens.refreshToken);
      setSession({ status: "signedIn", startedBy, ownerFullName });
    },
    [],
  );

  const signIn = useCallback(
    async (request: SignInRequest) => startSession(await signInRequest(request), "signIn"),
    [startSession],
  );

  const signUp = useCallback(
    async (request: SignUpRequest) =>
      startSession(await signUpRequest(request), "signUp", request.ownerFullName),
    [startSession],
  );

  const acceptInvitation = useCallback(
    async (request: AcceptInvitationRequest) =>
      startSession(await acceptInvitationRequest(request), "invitation"),
    [startSession],
  );

  const switchBranch = useCallback(
    async (businessId: string) => {
      const storedRefreshToken = await refreshTokenStorage.read();
      if (storedRefreshToken === null) {
        throw new MissingRefreshTokenError();
      }
      const tokens = await switchBranchRequest({ refreshToken: storedRefreshToken, businessId });
      await startSession(tokens, "branchSwitch");
      await queryClient.resetQueries();
    },
    [queryClient, startSession],
  );

  const signOut = useCallback(async () => {
    const storedRefreshToken = await refreshTokenStorage.read();
    if (storedRefreshToken !== null) {
      await signOutRequest({ refreshToken: storedRefreshToken }).catch(() => undefined);
    }
    authenticationSession.endSession();
  }, []);

  const contextValue = useMemo(
    () => ({ session, signIn, signUp, acceptInvitation, switchBranch, signOut }),
    [session, signIn, signUp, acceptInvitation, switchBranch, signOut],
  );

  return <SessionContext.Provider value={contextValue}>{children}</SessionContext.Provider>;
}

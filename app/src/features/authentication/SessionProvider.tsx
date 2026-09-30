import { useQueryClient } from "@tanstack/react-query";
import { createContext, PropsWithChildren, useCallback, useEffect, useMemo, useState } from "react";
import { authenticationSession } from "@/api/authenticationSession";
import {
  acceptFamilyInvitation as acceptFamilyInvitationRequest,
  acceptInvitation as acceptInvitationRequest,
  refreshSession,
  signIn as signInRequest,
  listAccounts,
  signOut as signOutRequest,
  signUp as signUpRequest,
  switchBranch as switchBranchRequest,
} from "@/features/authentication/authenticationApi";
import { SessionKind, sessionKindOf } from "@/features/authentication/sessionKind";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import {
  AcceptFamilyInvitationRequest,
  AcceptInvitationRequest,
  SignInRequest,
  SignUpRequest,
  TokenResponse,
} from "@/features/authentication/types";

export type SessionStartReason =
  | "signUp"
  | "signIn"
  | "invitation"
  | "familyInvitation"
  | "branchSwitch"
  | "accountSwitch"
  | "restore";

export type SessionState =
  | { status: "restoring" }
  | { status: "signedOut" }
  | {
      status: "signedIn";
      startedBy: SessionStartReason;
      kind: SessionKind;
      ownerFullName?: string;
      mustChooseAccount?: boolean;
    };

export interface SessionContextValue {
  session: SessionState;
  signIn: (request: SignInRequest) => Promise<void>;
  signUp: (request: SignUpRequest) => Promise<void>;
  acceptInvitation: (request: AcceptInvitationRequest) => Promise<void>;
  acceptFamilyInvitation: (request: AcceptFamilyInvitationRequest) => Promise<void>;
  switchBranch: (businessId: string) => Promise<void>;
  switchAccount: (businessId: string, kind: SessionKind) => Promise<void>;
  confirmAccount: () => void;
  signOut: () => Promise<void>;
}

class MissingRefreshTokenError extends Error {
  constructor() {
    super("No refresh token is stored");
    this.name = "MissingRefreshTokenError";
  }
}

const signedOutSession: SessionState = { status: "signedOut" };

async function hasTeamAndFamilyAccounts(): Promise<boolean> {
  const accounts = await listAccounts().catch(() => undefined);
  return (
    accounts !== undefined &&
    accounts.some((account) => account.kind === "team") &&
    accounts.some((account) => account.kind === "family")
  );
}

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
      : { status: "signedIn", startedBy: "restore", kind: sessionKindOf(restoredAccessToken) };
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
    async (
      tokens: TokenResponse,
      startedBy: SessionStartReason,
      ownerFullName?: string,
      askForAccount: boolean = false,
    ) => {
      authenticationSession.startSession(tokens.accessToken);
      await refreshTokenStorage.write(tokens.refreshToken);
      const mustChooseAccount = askForAccount && (await hasTeamAndFamilyAccounts());
      setSession({
        status: "signedIn",
        startedBy,
        kind: sessionKindOf(tokens.accessToken),
        ownerFullName,
        mustChooseAccount,
      });
    },
    [],
  );

  const signIn = useCallback(
    async (request: SignInRequest) =>
      startSession(await signInRequest(request), "signIn", undefined, true),
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

  const acceptFamilyInvitation = useCallback(
    async (request: AcceptFamilyInvitationRequest) =>
      startSession(await acceptFamilyInvitationRequest(request), "familyInvitation"),
    [startSession],
  );

  const switchTo = useCallback(
    async (businessId: string, kind: SessionKind | undefined, startedBy: SessionStartReason) => {
      const storedRefreshToken = await refreshTokenStorage.read();
      if (storedRefreshToken === null) {
        throw new MissingRefreshTokenError();
      }
      const tokens = await switchBranchRequest(
        kind === undefined
          ? { refreshToken: storedRefreshToken, businessId }
          : { refreshToken: storedRefreshToken, businessId, kind },
      );
      await startSession(tokens, startedBy);
      await queryClient.resetQueries();
    },
    [queryClient, startSession],
  );

  const switchBranch = useCallback(
    (businessId: string) => switchTo(businessId, undefined, "branchSwitch"),
    [switchTo],
  );

  const switchAccount = useCallback(
    (businessId: string, kind: SessionKind) => switchTo(businessId, kind, "accountSwitch"),
    [switchTo],
  );

  const confirmAccount = useCallback(
    () =>
      setSession((current) =>
        current.status === "signedIn" ? { ...current, mustChooseAccount: false } : current,
      ),
    [],
  );

  const signOut = useCallback(async () => {
    const storedRefreshToken = await refreshTokenStorage.read();
    if (storedRefreshToken !== null) {
      await signOutRequest({ refreshToken: storedRefreshToken }).catch(() => undefined);
    }
    authenticationSession.endSession();
  }, []);

  const contextValue = useMemo(
    () => ({
      session,
      signIn,
      signUp,
      acceptInvitation,
      acceptFamilyInvitation,
      switchBranch,
      switchAccount,
      confirmAccount,
      signOut,
    }),
    [
      session,
      signIn,
      signUp,
      acceptInvitation,
      acceptFamilyInvitation,
      switchBranch,
      switchAccount,
      confirmAccount,
      signOut,
    ],
  );

  return <SessionContext.Provider value={contextValue}>{children}</SessionContext.Provider>;
}

import { isNetworkError } from "@/api/apiErrors";

export interface AuthenticationSessionHandlers {
  refreshAccessToken: () => Promise<string>;
  onSessionEnded: () => void;
}

let accessToken: string | null = null;
let sessionHandlers: AuthenticationSessionHandlers | null = null;
let pendingRefresh: Promise<string | null> | null = null;

function endSession(): void {
  accessToken = null;
  sessionHandlers?.onSessionEnded();
}

async function runRefresh(): Promise<string | null> {
  if (sessionHandlers === null) {
    endSession();
    return null;
  }
  try {
    accessToken = await sessionHandlers.refreshAccessToken();
    return accessToken;
  } catch (refreshError) {
    if (isNetworkError(refreshError)) {
      throw refreshError;
    }
    endSession();
    return null;
  }
}

export const authenticationSession = {
  configure(handlers: AuthenticationSessionHandlers): void {
    sessionHandlers = handlers;
  },
  startSession(newAccessToken: string): void {
    accessToken = newAccessToken;
  },
  getAccessToken(): string | null {
    return accessToken;
  },
  refreshAccessToken(): Promise<string | null> {
    pendingRefresh ??= runRefresh().finally(() => {
      pendingRefresh = null;
    });
    return pendingRefresh;
  },
  endSession,
  reset(): void {
    accessToken = null;
    sessionHandlers = null;
    pendingRefresh = null;
  },
};

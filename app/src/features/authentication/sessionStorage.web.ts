import {
  cookieSessionStorageKey,
  refreshTokenStorageKey,
} from "@/features/authentication/sessionStorageKeys";
import { RefreshTokenStorage } from "@/features/authentication/types";

const cookieSessionMarker = "true";

function readItem(key: string): string | null {
  return globalThis.localStorage?.getItem(key) ?? null;
}

export const refreshTokenStorage: RefreshTokenStorage = {
  read: async () => {
    const refreshTokenFromBeforeTheCookie = readItem(refreshTokenStorageKey);
    if (refreshTokenFromBeforeTheCookie !== null) {
      return { refreshToken: refreshTokenFromBeforeTheCookie };
    }
    return readItem(cookieSessionStorageKey) === null ? null : {};
  },
  write: async () => {
    globalThis.localStorage?.removeItem(refreshTokenStorageKey);
    globalThis.localStorage?.setItem(cookieSessionStorageKey, cookieSessionMarker);
  },
  clear: async () => {
    globalThis.localStorage?.removeItem(refreshTokenStorageKey);
    globalThis.localStorage?.removeItem(cookieSessionStorageKey);
  },
};

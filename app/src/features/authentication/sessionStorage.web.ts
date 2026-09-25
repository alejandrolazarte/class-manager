import { refreshTokenStorageKey } from "@/features/authentication/sessionStorageKeys";
import { RefreshTokenStorage } from "@/features/authentication/types";

export const refreshTokenStorage: RefreshTokenStorage = {
  read: async () => globalThis.localStorage?.getItem(refreshTokenStorageKey) ?? null,
  write: async (refreshToken) => {
    globalThis.localStorage?.setItem(refreshTokenStorageKey, refreshToken);
  },
  clear: async () => {
    globalThis.localStorage?.removeItem(refreshTokenStorageKey);
  },
};

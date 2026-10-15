import * as SecureStore from "expo-secure-store";
import { refreshTokenStorageKey } from "@/features/authentication/sessionStorageKeys";
import { RefreshTokenStorage } from "@/features/authentication/types";

export const refreshTokenStorage: RefreshTokenStorage = {
  read: async () => {
    const storedRefreshToken = await SecureStore.getItemAsync(refreshTokenStorageKey);
    return storedRefreshToken === null ? null : { refreshToken: storedRefreshToken };
  },
  write: async ({ refreshToken }) => {
    if (refreshToken !== undefined) {
      await SecureStore.setItemAsync(refreshTokenStorageKey, refreshToken);
    }
  },
  clear: () => SecureStore.deleteItemAsync(refreshTokenStorageKey),
};

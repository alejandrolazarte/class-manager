import * as SecureStore from "expo-secure-store";
import { refreshTokenStorageKey } from "@/features/authentication/sessionStorageKeys";
import { RefreshTokenStorage } from "@/features/authentication/types";

export const refreshTokenStorage: RefreshTokenStorage = {
  read: () => SecureStore.getItemAsync(refreshTokenStorageKey),
  write: (refreshToken) => SecureStore.setItemAsync(refreshTokenStorageKey, refreshToken),
  clear: () => SecureStore.deleteItemAsync(refreshTokenStorageKey),
};

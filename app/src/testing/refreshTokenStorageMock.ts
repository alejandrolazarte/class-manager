import { refreshTokenStorage } from "@/features/authentication/sessionStorage";

export function mockRefreshTokenStorage(initiallyStoredToken: string | null): void {
  let storedToken = initiallyStoredToken;
  jest.mocked(refreshTokenStorage.read).mockImplementation(async () => storedToken);
  jest.mocked(refreshTokenStorage.write).mockImplementation(async (refreshToken) => {
    storedToken = refreshToken;
  });
  jest.mocked(refreshTokenStorage.clear).mockImplementation(async () => {
    storedToken = null;
  });
}

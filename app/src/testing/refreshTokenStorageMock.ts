import { refreshTokenStorage } from "@/features/authentication/sessionStorage";

export function mockRefreshTokenStorage(initiallyStoredToken: string | null): void {
  let storedToken = initiallyStoredToken;
  jest
    .mocked(refreshTokenStorage.read)
    .mockImplementation(async () => (storedToken === null ? null : { refreshToken: storedToken }));
  jest.mocked(refreshTokenStorage.write).mockImplementation(async ({ refreshToken }) => {
    storedToken = refreshToken ?? null;
  });
  jest.mocked(refreshTokenStorage.clear).mockImplementation(async () => {
    storedToken = null;
  });
}

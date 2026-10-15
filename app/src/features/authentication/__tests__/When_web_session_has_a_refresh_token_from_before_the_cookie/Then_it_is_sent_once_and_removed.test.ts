import { refreshTokenStorage } from "@/features/authentication/sessionStorage.web";
import { refreshTokenStorageKey } from "@/features/authentication/sessionStorageKeys";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { installInMemoryLocalStorage } from "@/testing/localStorageMock";

describe("When web session has a refresh token from before the cookie", () => {
  it("Then it is sent once and removed", async () => {
    const storedItems = installInMemoryLocalStorage();
    storedItems.set(refreshTokenStorageKey, storedRefreshToken);
    const tokensSentToTheBody = await refreshTokenStorage.read();

    await refreshTokenStorage.write(buildTokenResponse({ refreshToken: undefined }));

    expect(tokensSentToTheBody).toEqual({ refreshToken: storedRefreshToken });
    expect(storedItems.has(refreshTokenStorageKey)).toBe(false);
    expect(await refreshTokenStorage.read()).toEqual({});
  });
});

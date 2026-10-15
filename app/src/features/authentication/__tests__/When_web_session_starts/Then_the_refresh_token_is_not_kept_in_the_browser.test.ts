import { refreshTokenStorage } from "@/features/authentication/sessionStorage.web";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { installInMemoryLocalStorage } from "@/testing/localStorageMock";

describe("When web session starts", () => {
  it("Then the refresh token is not kept in the browser", async () => {
    const storedItems = installInMemoryLocalStorage();
    const issuedTokens = buildTokenResponse();

    await refreshTokenStorage.write(issuedTokens);

    expect([...storedItems.values()]).not.toContain(issuedTokens.refreshToken);
    expect(await refreshTokenStorage.read()).toEqual({});
  });
});

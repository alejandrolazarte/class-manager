import { refreshTokenStorage } from "@/features/authentication/sessionStorage.web";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { installInMemoryLocalStorage } from "@/testing/localStorageMock";

describe("When web session is cleared", () => {
  it("Then no session is restored", async () => {
    installInMemoryLocalStorage();
    await refreshTokenStorage.write(buildTokenResponse());

    await refreshTokenStorage.clear();

    expect(await refreshTokenStorage.read()).toBeNull();
  });
});

import { screen } from "@testing-library/react-native";
import { refreshSession } from "@/features/authentication/authenticationApi";
import { buildFamilyAccessToken } from "@/testing/familyFactory";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionProbe } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When family session is restored", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest
      .mocked(refreshSession)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildFamilyAccessToken() }));
  });

  it("Then session kind is family", async () => {
    await renderWithSession(<SessionProbe />);

    expect(await screen.findByText("signedIn")).toBeOnTheScreen();
    expect(screen.getByText("family")).toBeOnTheScreen();
  });
});

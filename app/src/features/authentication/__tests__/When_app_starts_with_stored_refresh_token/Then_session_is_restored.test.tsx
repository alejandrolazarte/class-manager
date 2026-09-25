import { screen } from "@testing-library/react-native";
import { authenticationSession } from "@/api/authenticationSession";
import { refreshSession } from "@/features/authentication/authenticationApi";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionProbe } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const rotatedTokens = buildTokenResponse({
  accessToken: "restored-access-token",
  refreshToken: "rotated-refresh-token",
});

describe("When app starts with stored refresh token", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(rotatedTokens);
  });

  it("Then session is restored", async () => {
    await renderWithSession(<SessionProbe />);

    expect(await screen.findByText("signedIn")).toBeOnTheScreen();
    expect(refreshSession).toHaveBeenCalledWith({ refreshToken: storedRefreshToken });
    expect(refreshTokenStorage.write).toHaveBeenCalledWith(rotatedTokens.refreshToken);
    expect(authenticationSession.getAccessToken()).toBe(rotatedTokens.accessToken);
  });
});

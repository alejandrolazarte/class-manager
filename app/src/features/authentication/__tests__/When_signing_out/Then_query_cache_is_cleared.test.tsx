import { fireEvent, screen } from "@testing-library/react-native";
import { refreshSession, signOut } from "@/features/authentication/authenticationApi";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { buildClient } from "@/testing/clientFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionProbe, signOutProbeLabel } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const rotatedTokens = buildTokenResponse({ refreshToken: "rotated-refresh-token" });

describe("When signing out", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(rotatedTokens);
    jest.mocked(signOut).mockResolvedValue();
  });

  it("Then query cache is cleared", async () => {
    const { queryClient } = await renderWithSession(<SessionProbe />);
    await screen.findByText("signedIn");
    queryClient.setQueryData(["clients", "detail", buildClient().id], buildClient());

    await fireEvent.press(screen.getByRole("button", { name: signOutProbeLabel }));

    expect(await screen.findByText("signedOut")).toBeOnTheScreen();
    expect(queryClient.getQueryCache().getAll()).toHaveLength(0);
    expect(signOut).toHaveBeenCalledWith({ refreshToken: rotatedTokens.refreshToken });
  });
});

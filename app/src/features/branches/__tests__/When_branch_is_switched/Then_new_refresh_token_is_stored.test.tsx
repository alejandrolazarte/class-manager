import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { refreshSession, switchBranch } from "@/features/authentication/authenticationApi";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { probeBranchId, SessionProbe, switchBranchProbeLabel } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const restoredTokens = buildTokenResponse({ refreshToken: "restored-refresh-token" });
const branchTokens = buildTokenResponse({ refreshToken: "branch-refresh-token" });

describe("When branch is switched", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(restoredTokens);
    jest.mocked(switchBranch).mockResolvedValue(branchTokens);
  });

  it("Then new refresh token is stored", async () => {
    await renderWithSession(<SessionProbe />);
    await screen.findByText("signedIn");

    await fireEvent.press(screen.getByRole("button", { name: switchBranchProbeLabel }));

    await waitFor(() =>
      expect(switchBranch).toHaveBeenCalledWith({
        refreshToken: restoredTokens.refreshToken,
        businessId: probeBranchId,
      }),
    );
    await waitFor(async () =>
      expect(await refreshTokenStorage.read()).toBe(branchTokens.refreshToken),
    );
  });
});

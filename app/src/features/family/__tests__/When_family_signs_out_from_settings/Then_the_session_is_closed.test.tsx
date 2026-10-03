import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { refreshSession, signOut } from "@/features/authentication/authenticationApi";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { buildFamilyAccessToken, buildFamilyHome } from "@/testing/familyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/family/familyApi");

const familyTokens = buildTokenResponse({ accessToken: buildFamilyAccessToken() });

describe("When family signs out from settings", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(familyTokens);
    jest.mocked(signOut).mockResolvedValue();
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
  });

  it("Then the session is closed", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("settings.signOut") }),
    );

    await waitFor(() =>
      expect(signOut).toHaveBeenCalledWith({ refreshToken: familyTokens.refreshToken }),
    );
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { refreshSession, signOut } from "@/features/authentication/authenticationApi";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { buildStudentAccessToken, buildStudentAppHome } from "@/testing/studentAppFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/studentApp/studentAppApi");

const studentTokens = buildTokenResponse({ accessToken: buildStudentAccessToken() });

describe("When student signs out from settings", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(studentTokens);
    jest.mocked(signOut).mockResolvedValue();
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
  });

  it("Then the session is closed", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("settings.signOut") }),
    );

    await waitFor(() =>
      expect(signOut).toHaveBeenCalledWith({ refreshToken: studentTokens.refreshToken }),
    );
  });
});

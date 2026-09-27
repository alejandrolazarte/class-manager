import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { resetPassword } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { ResetPasswordScreen } from "@/features/authentication/screens/ResetPasswordScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When reset link was already used", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "used-reset-token" };
    jest.mocked(resetPassword).mockRejectedValue(
      new ApiError(badRequestStatus, {
        code: authenticationErrorCodes.invalidPasswordResetToken,
      }),
    );
  });

  it("Then a new link is requested", async () => {
    await renderWithSession(<ResetPasswordScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.resetPassword.newPassword")),
      "a brand new passphrase",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.resetPassword.submit") }),
    );

    expect(
      await screen.findByText(translate("authentication.resetPassword.invalidLink")),
    ).toBeOnTheScreen();
    expect(
      screen.queryByLabelText(translate("authentication.resetPassword.newPassword")),
    ).not.toBeOnTheScreen();
  });
});

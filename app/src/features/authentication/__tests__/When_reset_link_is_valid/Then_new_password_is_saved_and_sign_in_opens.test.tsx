import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { resetPassword } from "@/features/authentication/authenticationApi";
import { ResetPasswordScreen } from "@/features/authentication/screens/ResetPasswordScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const resetToken = "reset-token-from-the-link";
const newPassword = "a brand new passphrase";

describe("When reset link is valid", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: resetToken };
    jest.mocked(resetPassword).mockResolvedValue(undefined);
  });

  it("Then new password is saved and sign in opens", async () => {
    await renderWithSession(<ResetPasswordScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.resetPassword.newPassword")),
      newPassword,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.resetPassword.submit") }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.signIn));
    expect(resetPassword).toHaveBeenCalledWith({ token: resetToken, newPassword });
  });
});

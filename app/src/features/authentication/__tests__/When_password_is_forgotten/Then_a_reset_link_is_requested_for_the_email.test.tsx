import { fireEvent, screen } from "@testing-library/react-native";
import { requestPasswordReset } from "@/features/authentication/authenticationApi";
import { ForgotPasswordScreen } from "@/features/authentication/screens/ForgotPasswordScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { ownerEmail } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When password is forgotten", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { email: ownerEmail };
    jest.mocked(requestPasswordReset).mockResolvedValue(undefined);
  });

  it("Then a reset link is requested for the email", async () => {
    await renderWithSession(<ForgotPasswordScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("authentication.forgotPassword.submit"),
      }),
    );

    expect(
      await screen.findByText(
        translate("authentication.forgotPassword.sent", { email: ownerEmail }),
      ),
    ).toBeOnTheScreen();
    expect(requestPasswordReset).toHaveBeenCalledWith({ email: ownerEmail });
  });
});

import { screen } from "@testing-library/react-native";
import { ResetPasswordScreen } from "@/features/authentication/screens/ResetPasswordScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const accountEmail = "laura@example.com";

describe("When reset link has the email", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "reset-token-from-the-link", email: accountEmail };
  });

  it("Then the email is offered as the username", async () => {
    await renderWithSession(<ResetPasswordScreen />);
    await screen.findByLabelText(translate("authentication.resetPassword.newPassword"));

    expect(
      screen.getByDisplayValue(accountEmail, { includeHiddenElements: true }).props.autoComplete,
    ).toBe("username");
  });
});

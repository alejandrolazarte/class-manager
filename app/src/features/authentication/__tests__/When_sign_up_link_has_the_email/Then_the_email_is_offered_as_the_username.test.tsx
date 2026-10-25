import { screen } from "@testing-library/react-native";
import { CompleteSignUpScreen } from "@/features/authentication/screens/CompleteSignUpScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const ownerEmail = "laura@example.com";

describe("When sign up link has the email", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "sign-up-token-from-the-link", email: ownerEmail };
  });

  it("Then the email is offered as the username", async () => {
    await renderWithSession(<CompleteSignUpScreen />);
    await screen.findByLabelText(translate("authentication.completeSignUp.password"));

    expect(
      screen.getByDisplayValue(ownerEmail, { includeHiddenElements: true }).props.autoComplete,
    ).toBe("username");
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { completeSignUp } from "@/features/authentication/authenticationApi";
import { CompleteSignUpScreen } from "@/features/authentication/screens/CompleteSignUpScreen";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { ownerPassword } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const signUpToken = "sign-up-token-from-the-link";
const issuedTokens = buildTokenResponse();

describe("When sign up link is valid", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: signUpToken };
    jest.mocked(completeSignUp).mockResolvedValue(issuedTokens);
  });

  it("Then home is shown", async () => {
    await renderWithSession(<CompleteSignUpScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.completeSignUp.password")),
      ownerPassword,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.completeSignUp.submit") }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(completeSignUp).toHaveBeenCalledWith({ token: signUpToken, password: ownerPassword });
    expect(refreshTokenStorage.write).toHaveBeenCalledWith(issuedTokens.refreshToken);
  });
});

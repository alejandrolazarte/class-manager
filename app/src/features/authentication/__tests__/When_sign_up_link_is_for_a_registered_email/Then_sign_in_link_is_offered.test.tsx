import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { completeSignUp } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { CompleteSignUpScreen } from "@/features/authentication/screens/CompleteSignUpScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { ownerPassword } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const conflictStatus = 409;

describe("When sign up link is for a registered email", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "sign-up-token" };
    jest
      .mocked(completeSignUp)
      .mockRejectedValue(
        new ApiError(conflictStatus, { code: authenticationErrorCodes.emailTaken }),
      );
  });

  it("Then sign in link is offered", async () => {
    await renderWithSession(<CompleteSignUpScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.completeSignUp.password")),
      ownerPassword,
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.completeSignUp.submit") }),
    );

    expect(
      await screen.findByText(translate("authentication.signUp.emailTaken")),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.signUp.signInWithEmail") }),
    );
    expect(routerMock.replace).toHaveBeenCalledWith(routes.signIn);
  });
});

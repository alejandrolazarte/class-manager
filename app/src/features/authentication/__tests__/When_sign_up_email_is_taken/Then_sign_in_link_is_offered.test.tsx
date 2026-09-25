import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { signUp } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { fillSignUpForm, ownerEmail, submitSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const conflictStatus = 409;

describe("When sign up email is taken", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest
      .mocked(signUp)
      .mockRejectedValue(
        new ApiError(conflictStatus, { code: authenticationErrorCodes.emailTaken }),
      );
  });

  it("Then sign in link is offered", async () => {
    await renderWithSession(<SignUpScreen />);
    await fillSignUpForm();
    await submitSignUpForm();

    expect(
      await screen.findByText(translate("authentication.signUp.emailTaken")),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.signUp.signInWithEmail") }),
    );
    expect(routerMock.replace).toHaveBeenCalledWith({
      pathname: "/sign-in",
      params: { email: ownerEmail },
    });
  });
});

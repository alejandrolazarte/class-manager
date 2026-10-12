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

const badRequestStatus = 400;

describe("When sign up link was already used", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "used-sign-up-token" };
    jest
      .mocked(completeSignUp)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: authenticationErrorCodes.invalidSignUpToken }),
      );
  });

  it("Then signing up again is offered", async () => {
    await renderWithSession(<CompleteSignUpScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.completeSignUp.password")),
      ownerPassword,
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.completeSignUp.submit") }),
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("authentication.completeSignUp.signUpAgain"),
      }),
    );

    expect(
      screen.getByText(translate("authentication.completeSignUp.invalidLink")),
    ).toBeOnTheScreen();
    expect(routerMock.replace).toHaveBeenCalledWith(routes.signUp);
  });
});

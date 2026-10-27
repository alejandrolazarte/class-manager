import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { signIn } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { translate } from "@/i18n/translate";
import { fillSignInForm, submitSignInForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const forbiddenStatus = 403;

describe("When the account has no business left", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest
      .mocked(signIn)
      .mockRejectedValue(
        new ApiError(forbiddenStatus, { code: authenticationErrorCodes.noBusinessAccess }),
      );
  });

  it("Then the no access message is shown", async () => {
    await renderWithSession(<SignInScreen />);
    await fillSignInForm();
    await submitSignInForm();

    expect(
      await screen.findByText(translate("authentication.signIn.noBusinessAccess")),
    ).toBeOnTheScreen();
  });
});

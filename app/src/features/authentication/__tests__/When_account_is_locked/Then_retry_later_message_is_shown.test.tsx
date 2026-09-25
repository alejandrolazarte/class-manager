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

const lockedStatus = 423;

describe("When account is locked", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest
      .mocked(signIn)
      .mockRejectedValue(new ApiError(lockedStatus, { code: authenticationErrorCodes.lockedOut }));
  });

  it("Then retry later message is shown", async () => {
    await renderWithSession(<SignInScreen />);
    await fillSignInForm();
    await submitSignInForm();

    expect(await screen.findByText(translate("authentication.signIn.lockedOut"))).toBeOnTheScreen();
  });
});

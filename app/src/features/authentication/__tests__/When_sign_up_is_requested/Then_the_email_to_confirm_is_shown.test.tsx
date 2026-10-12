import { screen } from "@testing-library/react-native";
import { requestSignUp } from "@/features/authentication/authenticationApi";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import { translate } from "@/i18n/translate";
import { fillSignUpForm, ownerEmail, submitSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When sign up is requested", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(requestSignUp).mockResolvedValue(undefined);
  });

  it("Then the email to confirm is shown", async () => {
    await renderWithSession(<SignUpScreen />);
    await fillSignUpForm();
    await submitSignUpForm();

    expect(
      await screen.findByText(translate("authentication.signUp.checkEmail", { email: ownerEmail })),
    ).toBeOnTheScreen();
    expect(requestSignUp).toHaveBeenCalledWith(expect.objectContaining({ email: ownerEmail }));
    expect(refreshTokenStorage.write).not.toHaveBeenCalled();
  });
});

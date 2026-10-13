import { fireEvent, screen } from "@testing-library/react-native";
import { requestSignUp } from "@/features/authentication/authenticationApi";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { fillSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When sign up legal terms are not accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(requestSignUp).mockResolvedValue(undefined);
  });

  it("Then the account cannot be created", async () => {
    await renderWithSession(<SignUpScreen />);
    await fillSignUpForm();
    await fireEvent.press(screen.getByRole("checkbox", { name: translate("legal.consent.label") }));

    const submit = screen.getByRole("button", { name: translate("authentication.signUp.submit") });
    await fireEvent.press(submit);

    expect(submit).toBeDisabled();
    expect(requestSignUp).not.toHaveBeenCalled();
  });
});

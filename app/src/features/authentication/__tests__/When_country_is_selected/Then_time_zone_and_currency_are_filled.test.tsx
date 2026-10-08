import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { signUp } from "@/features/authentication/authenticationApi";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { fillSignUpForm, submitSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When country is selected", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(signUp).mockResolvedValue(buildTokenResponse());
  });

  it("Then time zone and currency are filled", async () => {
    await renderWithSession(<SignUpScreen />);
    await fillSignUpForm();
    await fireEvent.press(screen.getByRole("button", { name: translate("countries.UY") }));
    await submitSignUpForm();

    await waitFor(() =>
      expect(signUp).toHaveBeenCalledWith(
        expect.objectContaining({
          timeZoneId: "America/Montevideo",
          currencyCode: "UYU",
          defaultCountryCallingCode: "598",
          ownerBirthDate: "1990-04-12",
        }),
      ),
    );
  });
});

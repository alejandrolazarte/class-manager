import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { requestSignUp } from "@/features/authentication/authenticationApi";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { fillSignUpForm, submitSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When country is selected", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(requestSignUp).mockResolvedValue(undefined);
  });

  it("Then time zone and currency are filled", async () => {
    await renderWithSession(<SignUpScreen />);
    await fillSignUpForm();
    await fireEvent.press(await screen.findByRole("button", { name: translate("countries.UY") }));
    await submitSignUpForm();

    await waitFor(() =>
      expect(requestSignUp).toHaveBeenCalledWith(
        expect.objectContaining({
          countryCode: "UY",
          timeZoneId: "America/Montevideo",
          currencyCode: "UYU",
          ownerBirthDate: "1990-04-12",
        }),
      ),
    );
  });
});

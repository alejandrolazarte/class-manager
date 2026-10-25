import { screen } from "@testing-library/react-native";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/passkeys/webAuthn");

describe("When passkeys are not supported on sign in", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(isPasskeySupported).mockResolvedValue(false);
  });

  it("Then no passkey button is shown", async () => {
    await renderWithSession(<SignInScreen />);

    expect(
      await screen.findByRole("button", { name: translate("authentication.signIn.submit") }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("authentication.signIn.withPasskey") }),
    ).not.toBeOnTheScreen();
  });
});

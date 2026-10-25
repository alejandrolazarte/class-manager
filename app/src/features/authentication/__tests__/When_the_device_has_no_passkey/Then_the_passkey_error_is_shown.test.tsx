import { fireEvent, screen } from "@testing-library/react-native";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { beginPasskeySignIn } from "@/features/passkeys/passkeysApi";
import { getPasskeyCredential, isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { buildPasskeySignInCeremony } from "@/testing/passkeyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

describe("When the device has no passkey", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(beginPasskeySignIn).mockResolvedValue(buildPasskeySignInCeremony());
    jest
      .mocked(getPasskeyCredential)
      .mockRejectedValue({ error: "NoCredentials", message: "No passkeys for this app" });
  });

  it("Then the passkey error is shown", async () => {
    await renderWithSession(<SignInScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("authentication.signIn.withPasskey") }),
    );

    expect(
      await screen.findByText(translate("authentication.signIn.passkeyNotVerified")),
    ).toBeOnTheScreen();
  });
});

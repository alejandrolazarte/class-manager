import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { beginPasskeySignIn, signInWithPasskey } from "@/features/passkeys/passkeysApi";
import { getPasskeyCredential, isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { buildPasskeySignInCeremony } from "@/testing/passkeyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

describe("When the native passkey prompt is cancelled", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(beginPasskeySignIn).mockResolvedValue(buildPasskeySignInCeremony());
    jest
      .mocked(getPasskeyCredential)
      .mockRejectedValue({ error: "UserCancelled", message: "The user cancelled the request" });
  });

  it("Then no error is shown", async () => {
    await renderWithSession(<SignInScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("authentication.signIn.withPasskey") }),
    );

    await waitFor(() => expect(getPasskeyCredential).toHaveBeenCalled());
    expect(signInWithPasskey).not.toHaveBeenCalled();
    expect(
      screen.queryByText(translate("authentication.signIn.passkeyNotVerified")),
    ).not.toBeOnTheScreen();
  });
});

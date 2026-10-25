import { fireEvent, screen } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { beginPasskeySignIn, signInWithPasskey } from "@/features/passkeys/passkeysApi";
import { getPasskeyCredential, isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { buildPasskeySignInCeremony, buildPasskeySignInCredential } from "@/testing/passkeyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionProbe } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

const ceremony = buildPasskeySignInCeremony();
const credential = buildPasskeySignInCredential();

describe("When signing in with a passkey", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(beginPasskeySignIn).mockResolvedValue(ceremony);
    jest.mocked(getPasskeyCredential).mockResolvedValue(credential);
    jest.mocked(signInWithPasskey).mockResolvedValue(buildTokenResponse());
    jest.mocked(listAccounts).mockResolvedValue([]);
  });

  it("Then the session starts", async () => {
    await renderWithSession(
      <>
        <SignInScreen />
        <SessionProbe />
      </>,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("authentication.signIn.withPasskey") }),
    );

    expect(await screen.findByText("signedIn")).toBeOnTheScreen();
    expect(getPasskeyCredential).toHaveBeenCalledWith(ceremony.options);
    expect(signInWithPasskey).toHaveBeenCalledWith({ ceremonyId: ceremony.ceremonyId, credential });
  });
});

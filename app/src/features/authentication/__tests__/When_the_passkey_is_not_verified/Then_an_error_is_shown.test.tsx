import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { beginPasskeySignIn, signInWithPasskey } from "@/features/passkeys/passkeysApi";
import { getPasskeyCredential, isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { buildPasskeySignInCeremony, buildPasskeySignInCredential } from "@/testing/passkeyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");
jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

const unauthorizedStatus = 401;

describe("When the passkey is not verified", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(beginPasskeySignIn).mockResolvedValue(buildPasskeySignInCeremony());
    jest.mocked(getPasskeyCredential).mockResolvedValue(buildPasskeySignInCredential());
    jest
      .mocked(signInWithPasskey)
      .mockRejectedValue(
        new ApiError(unauthorizedStatus, { code: authenticationErrorCodes.invalidCredentials }),
      );
  });

  it("Then an error is shown", async () => {
    await renderWithSession(<SignInScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("authentication.signIn.withPasskey") }),
    );

    expect(
      await screen.findByText(translate("authentication.signIn.passkeyNotVerified")),
    ).toBeOnTheScreen();
  });
});

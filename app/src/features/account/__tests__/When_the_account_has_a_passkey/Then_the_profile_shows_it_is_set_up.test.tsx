import { screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { listMyPasskeys } from "@/features/passkeys/passkeysApi";
import { isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildPasskey } from "@/testing/passkeyFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

describe("When the account has a passkey", () => {
  beforeEach(() => {
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(listMyPasskeys).mockResolvedValue([buildPasskey()]);
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
  });

  it("Then the profile shows it is set up", async () => {
    await renderWithProviders(
      <MyProfileScreen
        editProfileRoute={routes.editMyProfile}
        changeEmailRoute={routes.changeMyEmail}
        deleteAccountRoute={routes.deleteMyAccount}
        passkeysRoute={routes.myPasskeys}
      />,
    );

    expect(
      await screen.findByText(translateCount("profile.passkeys.configured", 1)),
    ).toBeOnTheScreen();
  });
});

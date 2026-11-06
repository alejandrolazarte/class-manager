import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { isPasskeySupported } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/passkeys/webAuthn");

describe("When passkeys are supported in the profile", () => {
  beforeEach(() => {
    jest.mocked(isPasskeySupported).mockResolvedValue(true);
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
  });

  it("Then the profile opens them", async () => {
    await renderWithProviders(
      <MyProfileScreen
        editProfileRoute={routes.editMyProfile}
        changeEmailRoute={routes.changeMyEmail}
        deleteAccountRoute={routes.deleteMyAccount}
        passkeysRoute={routes.myPasskeys}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("profile.passkeys.title") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.myPasskeys);
  });
});

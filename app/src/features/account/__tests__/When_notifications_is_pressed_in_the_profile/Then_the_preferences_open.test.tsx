import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When notifications is pressed in the profile", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
  });

  it("Then the preferences open", async () => {
    await renderWithProviders(
      <MyProfileScreen
        editProfileRoute={routes.editMyProfile}
        changeEmailRoute={routes.changeMyEmail}
        deleteAccountRoute={routes.deleteMyAccount}
        passkeysRoute={routes.myPasskeys}
        notificationPreferencesRoute={routes.myNotificationPreferences}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("notificationPreferences.title") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.myNotificationPreferences);
  });
});

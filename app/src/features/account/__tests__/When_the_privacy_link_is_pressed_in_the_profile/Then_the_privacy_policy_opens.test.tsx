import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the privacy link is pressed in the profile", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
  });

  it("Then the privacy policy opens", async () => {
    await renderWithProviders(
      <MyProfileScreen
        editProfileRoute={routes.editMyProfile}
        changeEmailRoute={routes.changeMyEmail}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("link", { name: translate("legal.privacy.title") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.legalDocument("privacy"));
  });
});

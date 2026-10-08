import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the profile is edited", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
    });
  });

  it("Then the edit screen opens", async () => {
    await renderWithProviders(<MyProfileScreen editProfileRoute={routes.studentAppEditProfile} />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("profile.editAccessibility") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.studentAppEditProfile);
  });
});

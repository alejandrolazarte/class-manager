import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyAvatarButton } from "@/features/userPhotos/components/MyAvatarButton";
import { getMyPhoto } from "@/features/userPhotos/userPhotoApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/userPhotos/userPhotoApi");

const photoUrl = "https://files.example.com/private/laura.webp?sig=abc";

describe("When my avatar is pressed", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
    jest
      .mocked(getMyPhoto)
      .mockResolvedValue({ url: photoUrl, awaitsGuardian: false, needsGuardian: false });
  });

  it("Then the profile opens", async () => {
    await renderWithProviders(<MyAvatarButton profileRoute={routes.myProfile} />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("profile.title") }));

    expect(routerMock.push).toHaveBeenCalledWith(routes.myProfile);
  });
});

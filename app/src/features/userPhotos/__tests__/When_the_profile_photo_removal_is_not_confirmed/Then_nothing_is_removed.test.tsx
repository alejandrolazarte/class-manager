import { fireEvent, screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { changeMyPhoto, getMyPhoto, removeMyPhoto } from "@/features/userPhotos/userPhotoApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/userPhotos/userPhotoApi");

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

function renderProfile() {
  return renderWithProviders(
    <MyProfileScreen
      editProfileRoute={routes.editMyProfile}
      changeEmailRoute={routes.changeMyEmail}
      deleteAccountRoute={routes.deleteMyAccount}
      passkeysRoute={routes.myPasskeys}
    />,
  );
}

function mockAccount() {
  jest.mocked(getMyAccount).mockResolvedValue({
    email: "tomas@example.com",
    fullName: "Tomas Perez",
    birthDate: "1990-04-12",
    hasAcceptedCurrentLegalTerms: true,
    preferredLanguage: "es",
  });
}

describe("When the profile photo removal is not confirmed", () => {
  beforeEach(() => {
    mockAccount();
    jest
      .mocked(getMyPhoto)
      .mockResolvedValue({ url: photoUrl, awaitsGuardian: false, needsGuardian: false });
  });

  it("Then nothing is removed", async () => {
    await renderProfile();

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("profile.photo.remove") }),
    );
    expect(await screen.findByText(translate("profile.photo.removeQuestion"))).toBeOnTheScreen();
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("common.back") }).at(-1)!,
    );

    expect(removeMyPhoto).not.toHaveBeenCalled();
    expect(changeMyPhoto).not.toHaveBeenCalled();
  });
});

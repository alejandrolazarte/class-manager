import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { getMyPhoto, removeMyPhoto } from "@/features/userPhotos/userPhotoApi";
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

describe("When the profile photo removal is confirmed", () => {
  beforeEach(() => {
    mockAccount();
    jest
      .mocked(getMyPhoto)
      .mockResolvedValue({ url: photoUrl, awaitsGuardian: false, needsGuardian: false });
    jest.mocked(removeMyPhoto).mockResolvedValue();
  });

  it("Then it is removed", async () => {
    await renderProfile();

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("profile.photo.remove") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("profile.photo.confirmRemove") }),
    );

    await waitFor(() => expect(removeMyPhoto).toHaveBeenCalled());
    expect(await screen.findByText(translate("profile.photo.removed"))).toBeOnTheScreen();
  });
});

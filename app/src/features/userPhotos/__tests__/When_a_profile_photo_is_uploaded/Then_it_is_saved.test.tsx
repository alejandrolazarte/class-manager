import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { pickUserPhoto } from "@/features/userPhotos/pickUserPhoto";
import { changeMyPhoto, getMyPhoto, removeMyPhoto } from "@/features/userPhotos/userPhotoApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/userPhotos/userPhotoApi");
jest.mock("@/features/userPhotos/pickUserPhoto", () => ({
  ...jest.requireActual("@/features/userPhotos/pickUserPhoto"),
  pickUserPhoto: jest.fn(),
}));

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";
const pickedPhoto = { name: "me.jpg", uri: "file:///me.jpg", mimeType: "image/jpeg", size: 2048 };

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

describe("When a profile photo is uploaded", () => {
  beforeEach(() => {
    mockAccount();
    jest
      .mocked(getMyPhoto)
      .mockResolvedValue({ url: null, awaitsGuardian: false, needsGuardian: false });
    jest.mocked(pickUserPhoto).mockResolvedValue(pickedPhoto);
    jest
      .mocked(changeMyPhoto)
      .mockResolvedValue({ url: photoUrl, awaitsGuardian: false, needsGuardian: false });
  });

  it("Then it is saved", async () => {
    await renderProfile();

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("profile.photo.upload") }),
    );

    await waitFor(() => expect(changeMyPhoto).toHaveBeenCalledWith(pickedPhoto));
    expect(await screen.findByText(translate("profile.photo.saved"))).toBeOnTheScreen();
    expect(removeMyPhoto).not.toHaveBeenCalled();
  });
});

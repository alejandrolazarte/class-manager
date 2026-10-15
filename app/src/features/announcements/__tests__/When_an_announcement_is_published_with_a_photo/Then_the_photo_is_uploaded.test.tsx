import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  addAnnouncementImage,
  createAnnouncement,
} from "@/features/announcements/announcementsApi";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
import { pickCatalogImage } from "@/features/catalogImages/pickCatalogImage";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { buildCatalogImages, buildPickedImage } from "@/testing/catalogImageFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/catalogImages/pickCatalogImage", () => ({
  ...jest.requireActual("@/features/catalogImages/pickCatalogImage"),
  pickCatalogImage: jest.fn(),
}));

const pickedImage = buildPickedImage();
const publishedAnnouncement = buildAnnouncement({ id: "new-announcement" });

describe("When an announcement is published with a photo", () => {
  beforeEach(() => {
    jest.mocked(pickCatalogImage).mockResolvedValue(pickedImage);
    jest.mocked(createAnnouncement).mockResolvedValue(publishedAnnouncement);
    jest
      .mocked(addAnnouncementImage)
      .mockResolvedValue({ ...publishedAnnouncement, images: buildCatalogImages(1) });
  });

  it("Then the photo is uploaded", async () => {
    await renderWithProviders(<NewAnnouncementScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Muestra de fin de año",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.add") }));
    await screen.findByTestId("catalog-image");

    await fireEvent.press(screen.getByRole("button", { name: translate("announcements.publish") }));

    await waitFor(() =>
      expect(addAnnouncementImage).toHaveBeenCalledWith("new-announcement", pickedImage),
    );
  });
});

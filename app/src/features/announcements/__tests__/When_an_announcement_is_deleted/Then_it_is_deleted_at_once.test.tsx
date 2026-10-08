import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteAnnouncement, listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = {
  id: "announcement-1",
  title: "Lunes 12 cerrado",
  body: null,
  publishedAt: "2026-09-30T10:00:00Z",
};

describe("When an announcement is deleted", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(deleteAnnouncement).mockResolvedValue(undefined);
  });

  it("Then it is deleted at once", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.delete", { title: announcement.title }),
      }),
    );

    await waitFor(() => expect(deleteAnnouncement).toHaveBeenCalledWith(announcement.id));
  });
});

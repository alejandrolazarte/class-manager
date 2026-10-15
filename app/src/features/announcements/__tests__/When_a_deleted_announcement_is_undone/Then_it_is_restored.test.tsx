import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  deleteAnnouncement,
  listAnnouncements,
  restoreAnnouncement,
} from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement();

describe("When a deleted announcement is undone", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(deleteAnnouncement).mockResolvedValue(undefined);
    jest.mocked(restoreAnnouncement).mockResolvedValue(undefined);
  });

  it("Then it is restored", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.delete", { title: announcement.title }),
      }),
    );

    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() => expect(restoreAnnouncement).toHaveBeenCalledWith(announcement.id));
  });
});

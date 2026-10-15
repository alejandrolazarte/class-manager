import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAnnouncements, unpinAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement({ isPinned: true });

describe("When a pinned announcement is unpinned", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(unpinAnnouncement).mockResolvedValue(undefined);
  });

  it("Then it is unpinned", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.unpin", { title: announcement.title }),
      }),
    );

    await waitFor(() => expect(unpinAnnouncement).toHaveBeenCalledWith(announcement.id));
  });
});

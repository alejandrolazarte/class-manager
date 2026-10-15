import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAnnouncements, pinAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement();

describe("When an announcement is pinned from the list", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(pinAnnouncement).mockResolvedValue(undefined);
  });

  it("Then it is pinned", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.pin", { title: announcement.title }),
      }),
    );

    await waitFor(() => expect(pinAnnouncement).toHaveBeenCalledWith(announcement.id));
  });
});

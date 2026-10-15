import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAnnouncements, updateAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

const publishOn = new Date(2099, 11, 5, 9, 0).toISOString();
const announcement = buildAnnouncement({ isScheduled: true, publishedAt: publishOn });

describe("When a scheduled announcement is edited", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(updateAnnouncement).mockResolvedValue(announcement);
  });

  it("Then it keeps its publish time", async () => {
    await renderWithProviders(<AnnouncementFormScreen announcementId={announcement.id} />);
    expect(await screen.findByDisplayValue("05/12/2099")).toBeOnTheScreen();
    expect(screen.getByDisplayValue("09:00")).toBeOnTheScreen();

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateAnnouncement).toHaveBeenCalledWith(
        announcement.id,
        expect.objectContaining({ publishOn, isPinned: false }),
      ),
    );
  });
});

import { screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement({ category: "News" });

describe("When an announcement is plain news", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
  });

  it("Then no category is shown", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await screen.findByText(announcement.title);
    expect(screen.queryByText(translate("announcements.category.news"))).not.toBeOnTheScreen();
  });
});

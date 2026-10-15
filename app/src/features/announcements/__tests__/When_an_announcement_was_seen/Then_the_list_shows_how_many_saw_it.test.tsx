import { screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

describe("When an announcement was seen", () => {
  beforeEach(() => {
    jest
      .mocked(listAnnouncements)
      .mockResolvedValue([buildAnnouncement({ recipientCount: 52, seenCount: 34 })]);
  });

  it("Then the list shows how many saw it", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    expect(
      await screen.findByText(translate("announcements.readsLine", { seen: 34, total: 52 })),
    ).toBeOnTheScreen();
  });
});

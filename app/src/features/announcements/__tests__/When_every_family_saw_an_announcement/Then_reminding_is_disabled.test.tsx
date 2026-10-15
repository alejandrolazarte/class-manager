import { screen } from "@testing-library/react-native";
import {
  listAnnouncementRecipients,
  listAnnouncements,
} from "@/features/announcements/announcementsApi";
import { AnnouncementDetailScreen } from "@/features/announcements/screens/AnnouncementDetailScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement({ recipientCount: 1, seenCount: 1 });

describe("When every family saw an announcement", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(listAnnouncementRecipients).mockResolvedValue([
      {
        clientId: "client-1",
        fullName: "Ana Castro",
        seenAt: "2026-09-30T12:00:00Z",
        isGoing: null,
      },
    ]);
  });

  it("Then reminding is disabled", async () => {
    await renderWithProviders(<AnnouncementDetailScreen announcementId={announcement.id} />);

    expect(
      await screen.findByRole("button", { name: translate("announcements.remind") }),
    ).toBeDisabled();
    expect(screen.getByText(translate("announcements.detail.emptyUnseen"))).toBeOnTheScreen();
  });
});

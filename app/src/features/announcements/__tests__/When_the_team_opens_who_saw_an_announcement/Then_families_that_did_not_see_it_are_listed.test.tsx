import { screen } from "@testing-library/react-native";
import {
  listAnnouncementRecipients,
  listAnnouncements,
} from "@/features/announcements/announcementsApi";
import { AnnouncementDetailScreen } from "@/features/announcements/screens/AnnouncementDetailScreen";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement({ recipientCount: 2, seenCount: 1 });

describe("When the team opens who saw an announcement", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(listAnnouncementRecipients).mockResolvedValue([
      { clientId: "client-1", fullName: "Ana Castro", seenAt: null, isGoing: null },
      {
        clientId: "client-2",
        fullName: "Bruno Sosa",
        seenAt: "2026-09-30T12:00:00Z",
        isGoing: null,
      },
    ]);
  });

  it("Then families that did not see it are listed", async () => {
    await renderWithProviders(<AnnouncementDetailScreen announcementId={announcement.id} />);

    expect(await screen.findByText("Ana Castro")).toBeOnTheScreen();
    expect(screen.queryByText("Bruno Sosa")).toBeNull();
  });
});

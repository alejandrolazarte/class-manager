import { fireEvent, screen } from "@testing-library/react-native";
import {
  listAnnouncementRecipients,
  listAnnouncements,
  remindAnnouncement,
} from "@/features/announcements/announcementsApi";
import { AnnouncementDetailScreen } from "@/features/announcements/screens/AnnouncementDetailScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement({ recipientCount: 1 });

describe("When the team reminds who did not see it", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest
      .mocked(listAnnouncementRecipients)
      .mockResolvedValue([
        { clientId: "client-1", fullName: "Ana Castro", seenAt: null, isGoing: null },
      ]);
    jest.mocked(remindAnnouncement).mockResolvedValue({ remindedCount: 1 });
  });

  it("Then the reminder is sent", async () => {
    await renderWithProviders(<AnnouncementDetailScreen announcementId={announcement.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("announcements.remind") }),
    );

    expect(await screen.findByText(translateCount("announcements.reminded", 1))).toBeOnTheScreen();
    expect(remindAnnouncement).toHaveBeenCalledWith(announcement.id);
  });
});

import { screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

const announcement = buildAnnouncement();

describe("When a published announcement is edited", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
  });

  it("Then its publish time is not offered", async () => {
    await renderWithProviders(<AnnouncementFormScreen announcementId={announcement.id} />);

    expect(await screen.findByDisplayValue(announcement.title)).toBeOnTheScreen();
    expect(screen.queryByText(translate("announcements.publishTime.label"))).toBeNull();
  });
});

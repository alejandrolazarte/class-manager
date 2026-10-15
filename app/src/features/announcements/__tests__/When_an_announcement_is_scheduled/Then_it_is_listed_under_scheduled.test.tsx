import { fireEvent, screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const published = buildAnnouncement({ id: "published", title: "Horarios de verano" });
const scheduled = buildAnnouncement({
  id: "scheduled",
  title: "Feriado del 8 de diciembre",
  isScheduled: true,
});

describe("When an announcement is scheduled", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([published, scheduled]);
  });

  it("Then it is listed under scheduled", async () => {
    await renderWithProviders(<AnnouncementsScreen />);
    expect(await screen.findByText(published.title)).toBeOnTheScreen();
    expect(screen.queryByText(scheduled.title)).toBeNull();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.filter.scheduled") }),
    );

    expect(await screen.findByText(scheduled.title)).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("announcements.pin", { title: scheduled.title }),
      }),
    ).toBeNull();
  });
});

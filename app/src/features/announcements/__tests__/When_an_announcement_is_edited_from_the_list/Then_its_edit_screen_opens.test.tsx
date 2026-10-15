import { fireEvent, screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

const announcement = buildAnnouncement();

describe("When an announcement is edited from the list", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
  });

  it("Then its edit screen opens", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.edit", { title: announcement.title }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.editAnnouncement(announcement.id));
  });
});

import { screen } from "@testing-library/react-native";
import { listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

describe("When there are no announcements", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([]);
  });

  it("Then the new announcement button is shown", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    expect(await screen.findByText(translate("announcements.empty"))).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("announcements.newTitle") }),
    ).toBeOnTheScreen();
  });
});

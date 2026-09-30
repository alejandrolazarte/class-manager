import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createAnnouncement, listAnnouncements } from "@/features/announcements/announcementsApi";
import { AnnouncementsScreen } from "@/features/announcements/screens/AnnouncementsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");

describe("When owner publishes an announcement", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([]);
    jest.mocked(createAnnouncement).mockResolvedValue({
      id: "announcement-1",
      title: "Lunes 12 cerrado",
      body: null,
      publishedAt: "2026-09-30T10:00:00Z",
    });
  });

  it("Then it is sent", async () => {
    await renderWithProviders(<AnnouncementsScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Lunes 12 cerrado",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("announcements.publish") }));

    await waitFor(() =>
      expect(createAnnouncement).toHaveBeenCalledWith({ title: "Lunes 12 cerrado", body: null }),
    );
  });
});

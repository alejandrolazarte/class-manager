import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When owner publishes a pinned event", () => {
  beforeEach(() => {
    jest.mocked(createAnnouncement).mockResolvedValue(buildAnnouncement());
  });

  it("Then category and pin are sent", async () => {
    await renderWithProviders(<AnnouncementFormScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Muestra de fin de año",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.category.event") }),
    );
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("announcements.pinToggle") }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("announcements.publish") }));

    await waitFor(() =>
      expect(createAnnouncement).toHaveBeenCalledWith(
        expect.objectContaining({ category: "Event", isPinned: true }),
      ),
    );
  });
});

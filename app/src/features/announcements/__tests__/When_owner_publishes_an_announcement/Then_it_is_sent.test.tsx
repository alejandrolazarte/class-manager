import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createAnnouncement } from "@/features/announcements/announcementsApi";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When owner publishes an announcement", () => {
  beforeEach(() => {
    jest.mocked(createAnnouncement).mockResolvedValue(buildAnnouncement());
  });

  it("Then it is sent", async () => {
    await renderWithProviders(<NewAnnouncementScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Lunes 12 cerrado",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("announcements.publish") }));

    await waitFor(() =>
      expect(createAnnouncement).toHaveBeenCalledWith({
        title: "Lunes 12 cerrado",
        body: null,
        category: "News",
        classGroupIds: [],
        isPinned: false,
        visibleUntil: null,
      }),
    );
  });
});

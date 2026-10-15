import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAnnouncements, updateAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

const announcement = buildAnnouncement({ body: "Feriado.", category: "Important", isPinned: true });

describe("When owner edits an announcement", () => {
  beforeEach(() => {
    jest.mocked(listAnnouncements).mockResolvedValue([announcement]);
    jest.mocked(updateAnnouncement).mockResolvedValue(announcement);
  });

  it("Then the changes are sent", async () => {
    await renderWithProviders(<AnnouncementFormScreen announcementId={announcement.id} />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Lunes 12 y martes 13 cerrado",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateAnnouncement).toHaveBeenCalledWith(announcement.id, {
        title: "Lunes 12 y martes 13 cerrado",
        body: "Feriado.",
        category: "Important",
        classGroupIds: [],
        isPinned: true,
        visibleUntil: null,
        publishOn: null,
      }),
    );
  });
});

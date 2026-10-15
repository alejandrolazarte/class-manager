import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createAnnouncement } from "@/features/announcements/announcementsApi";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

const beginners = buildClassGroup({ id: "class-group-beginners", name: "Nivel inicial" });

describe("When an announcement is for some classes", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([beginners]);
    jest.mocked(createAnnouncement).mockResolvedValue(buildAnnouncement());
  });

  it("Then only those classes are sent", async () => {
    await renderWithProviders(<AnnouncementFormScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Cambia el horario",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.audience.classes") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("announcements.audience.chooseClassGroup"),
      }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: beginners.name }));

    await fireEvent.press(screen.getByRole("button", { name: translate("announcements.publish") }));

    await waitFor(() =>
      expect(createAnnouncement).toHaveBeenCalledWith(
        expect.objectContaining({ classGroupIds: [beginners.id] }),
      ),
    );
  });
});

import { fireEvent, screen } from "@testing-library/react-native";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When an announcement is for classes but none is chosen", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([buildClassGroup()]);
  });

  it("Then publish stays disabled", async () => {
    await renderWithProviders(<AnnouncementFormScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Cambia el horario",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.audience.classes") }),
    );

    expect(screen.getByRole("button", { name: translate("announcements.publish") })).toBeDisabled();
  });
});

import { fireEvent, screen } from "@testing-library/react-native";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
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
    await renderWithProviders(<NewAnnouncementScreen />);
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

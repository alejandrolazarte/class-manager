import { fireEvent, screen } from "@testing-library/react-native";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When visible until is in the past", () => {
  it("Then publish stays disabled", async () => {
    await renderWithProviders(<NewAnnouncementScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Feriado",
    );

    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.visibleUntil")),
      "01012020",
    );

    expect(screen.getByText(translate("announcements.validation.visibleUntil"))).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: translate("announcements.publish") })).toBeDisabled();
  });
});

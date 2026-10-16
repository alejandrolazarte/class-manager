import { fireEvent, screen } from "@testing-library/react-native";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When an announcement link is not https", () => {
  it("Then publish stays disabled", async () => {
    await renderWithProviders(<AnnouncementFormScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Inscripciones abiertas",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.action.link") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.action.url")),
      "example.com",
    );

    expect(screen.getByText(translate("announcements.validation.actionUrl"))).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: translate("announcements.publish") })).toBeDisabled();
  });
});

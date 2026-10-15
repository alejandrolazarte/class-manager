import { fireEvent, screen } from "@testing-library/react-native";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When the publish time has passed", () => {
  it("Then scheduling stays disabled", async () => {
    await renderWithProviders(<NewAnnouncementScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Feriado",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.publishTime.scheduled") }),
    );

    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.publishTime.date")),
      "01012020",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.publishTime.time")),
      "0900",
    );

    expect(
      screen.getByText(translate("announcements.validation.publishTimeInThePast")),
    ).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("announcements.schedule") }),
    ).toBeDisabled();
  });
});

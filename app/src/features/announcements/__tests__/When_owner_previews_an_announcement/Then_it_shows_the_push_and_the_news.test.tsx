import { fireEvent, screen, within } from "@testing-library/react-native";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

const business = buildBusiness();

describe("When owner previews an announcement", () => {
  it("Then it shows the push and the news", async () => {
    await renderWithProviders(<AnnouncementFormScreen />, { business });
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Lunes 12 cerrado",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.form.body")),
      "Las clases se recuperan durante la semana.",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.preview.open") }),
    );

    const push = await screen.findByTestId("push-notification-preview");
    expect(within(push).getByText(business.name)).toBeOnTheScreen();
    expect(within(push).getByText("Lunes 12 cerrado")).toBeOnTheScreen();
    expect(screen.getAllByText("Las clases se recuperan durante la semana.")).toHaveLength(2);
  });
});

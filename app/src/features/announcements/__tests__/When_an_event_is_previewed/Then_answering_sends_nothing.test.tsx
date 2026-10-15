import { fireEvent, screen } from "@testing-library/react-native";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";
import { replyToAnnouncement } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/studentApp/studentAppApi");

describe("When an event is previewed", () => {
  it("Then answering sends nothing", async () => {
    await renderWithProviders(<AnnouncementFormScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Muestra de fin de año",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.category.event") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.preview.open") }),
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.news.reply.going") }),
    );

    expect(replyToAnnouncement).not.toHaveBeenCalled();
  });
});

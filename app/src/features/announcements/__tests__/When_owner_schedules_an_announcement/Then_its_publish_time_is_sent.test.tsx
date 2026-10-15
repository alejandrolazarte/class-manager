import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createAnnouncement } from "@/features/announcements/announcementsApi";
import { NewAnnouncementScreen } from "@/features/announcements/screens/NewAnnouncementScreen";
import { translate } from "@/i18n/translate";
import { buildAnnouncement } from "@/testing/announcementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/announcements/announcementsApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When owner schedules an announcement", () => {
  beforeEach(() => {
    jest.mocked(createAnnouncement).mockResolvedValue(buildAnnouncement({ isScheduled: true }));
  });

  it("Then its publish time is sent", async () => {
    await renderWithProviders(<NewAnnouncementScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("announcements.form.title")),
      "Feriado del 8 de diciembre",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.publishTime.scheduled") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.publishTime.date")),
      "05122099",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("announcements.publishTime.time")),
      "0900",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("announcements.schedule") }),
    );

    await waitFor(() =>
      expect(createAnnouncement).toHaveBeenCalledWith(
        expect.objectContaining({ publishOn: new Date(2099, 11, 5, 9, 0).toISOString() }),
      ),
    );
  });
});

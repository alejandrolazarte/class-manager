import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When next month is pressed", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({ month: "2026-09", days: [] });
  });

  it("Then the same day next month is picked", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("home.view.month") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.calendar.nextMonth") }),
    );

    await waitFor(() => expect(listDaySessions).toHaveBeenLastCalledWith("2026-10-29"));
    expect(getMonthCalendar).toHaveBeenLastCalledWith("2026-10");
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When a day is picked in month view", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({
      month: "2026-09",
      days: [{ date: "2026-09-10", classCount: 2, cancelledCount: 0, pendingAttendanceCount: 0 }],
    });
  });

  it("Then that days sessions are requested", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.calendar.show") }),
    );

    await fireEvent.press(await screen.findByRole("button", { name: "Jueves 10 · 2 clases" }));

    await waitFor(() => expect(listDaySessions).toHaveBeenLastCalledWith("2026-09-10"));
    expect(
      screen.getByRole("button", { name: translate("sessions.calendar.show") }),
    ).toBeOnTheScreen();
  });
});

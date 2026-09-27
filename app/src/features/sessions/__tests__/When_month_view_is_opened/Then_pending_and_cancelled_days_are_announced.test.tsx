import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When month view is opened", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({
      month: "2026-09",
      days: [
        { date: "2026-09-10", classCount: 2, cancelledCount: 0, pendingAttendanceCount: 1 },
        { date: "2026-09-17", classCount: 1, cancelledCount: 1, pendingAttendanceCount: 0 },
      ],
    });
  });

  it("Then pending and cancelled days are announced", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.calendar.show") }),
    );

    await waitFor(() => expect(getMonthCalendar).toHaveBeenCalledWith("2026-09"));
    expect(
      await screen.findByRole("button", { name: "Jueves 10 · 2 clases · asistencia pendiente" }),
    ).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: "Jueves 17 · 1 clase · cancelada" }),
    ).toBeOnTheScreen();
  });
});

import { screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const dayBeforeTheClassExisted = "2026-09-28";

describe("When a new class has no past sessions", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({
      month: "2026-09",
      days: [{ date: sessionDate, classCount: 1, cancelledCount: 0, pendingAttendanceCount: 0 }],
    });
  });

  it("Then past days of the week have no class dot", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);

    expect(await screen.findByTestId(`day-strip-class-dot-${sessionDate}`)).toBeOnTheScreen();
    await waitFor(() => expect(getMonthCalendar).toHaveBeenCalledWith("2026-09"));
    expect(screen.queryByTestId(`day-strip-class-dot-${dayBeforeTheClassExisted}`)).toBeNull();
  });
});

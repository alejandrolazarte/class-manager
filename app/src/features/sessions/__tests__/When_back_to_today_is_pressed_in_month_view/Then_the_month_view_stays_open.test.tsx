import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { todayIsoDate } from "@/features/sessions/dates";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When back to today is pressed in month view", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({ month: "2026-09", days: [] });
  });

  it("Then the month view stays open", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("home.view.month") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.day.backToToday") }),
    );

    await waitFor(() => expect(listDaySessions).toHaveBeenLastCalledWith(todayIsoDate()));
    expect(screen.getByRole("button", { name: translate("home.view.month") })).toBeSelected();
  });
});

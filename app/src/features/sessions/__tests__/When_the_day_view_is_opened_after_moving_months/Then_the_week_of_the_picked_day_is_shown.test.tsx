import { fireEvent, screen } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { getMonthCalendar, listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When the day view is opened after moving months", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
    jest.mocked(getMonthCalendar).mockResolvedValue({ month: "2026-09", days: [] });
  });

  it("Then the week of the picked day is shown", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("home.view.month") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.calendar.nextMonth") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("home.view.day") }));

    expect(await screen.findByRole("button", { name: "Jueves 29" })).toBeSelected();
  });
});

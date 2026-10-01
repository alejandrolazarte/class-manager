import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When next week is pressed", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
  });

  it("Then the same day next week is requested", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await screen.findByRole("button", { name: "18:00 Natación inicial" });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.calendar.nextWeek") }),
    );

    await waitFor(() => expect(listDaySessions).toHaveBeenLastCalledWith("2026-10-06"));
  });
});

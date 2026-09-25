import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

describe("When next day is pressed", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([buildDaySession()]);
  });

  it("Then sessions of that day are requested", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);
    await screen.findByText("Natación inicial", { exact: false });

    await fireEvent.press(screen.getByRole("button", { name: translate("sessions.day.next") }));

    await waitFor(() => expect(listDaySessions).toHaveBeenLastCalledWith("2026-09-30"));
  });
});

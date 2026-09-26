import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, restoreSessionSchedule } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  startTime: "19:00",
  endTime: "19:45",
  originalStartTime: "18:00",
});

describe("When session was rescheduled", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(restoreSessionSchedule).mockResolvedValue(undefined);
  });

  it("Then original time and restore are shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(translate("sessions.reschedule.notice", { original: "18:00" })),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.reschedule.restore") }),
    );
    await waitFor(() =>
      expect(restoreSessionSchedule).toHaveBeenCalledWith(session.classGroupId, sessionDate),
    );
  });
});

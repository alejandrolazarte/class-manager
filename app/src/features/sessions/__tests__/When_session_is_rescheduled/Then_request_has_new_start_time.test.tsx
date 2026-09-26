import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, rescheduleSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails();

describe("When session is rescheduled", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(rescheduleSession).mockResolvedValue(undefined);
  });

  it("Then request has new start time", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.reschedule.open") }),
    );

    await fireEvent.changeText(
      screen.getByLabelText(translate("sessions.reschedule.startTime")),
      "1900",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.reschedule.confirm") }),
    );

    await waitFor(() =>
      expect(rescheduleSession).toHaveBeenCalledWith(session.classGroupId, sessionDate, "19:00"),
    );
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, restoreSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  isCancelled: true,
  cancellationReason: "Feriado",
  canTakeAttendance: false,
});

describe("When session is cancelled", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(restoreSession).mockResolvedValue(undefined);
  });

  it("Then banner and restore are shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(translate("sessions.session.cancelled", { reason: "Feriado" })),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.session.restore") }),
    );
    await waitFor(() =>
      expect(restoreSession).toHaveBeenCalledWith(session.classGroupId, sessionDate),
    );
  });
});

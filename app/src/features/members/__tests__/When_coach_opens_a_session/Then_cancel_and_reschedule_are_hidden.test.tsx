import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { buildCoach } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({ canReschedule: true });

describe("When coach opens a session", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then cancel and reschedule are hidden", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
      { member: buildCoach() },
    );

    expect(await screen.findByText(session.classGroupName)).toBeOnTheScreen();
    expect(screen.queryByText(translate("sessions.session.cancel"))).toBeNull();
    expect(screen.queryByText(translate("sessions.reschedule.open"))).toBeNull();
  });
});

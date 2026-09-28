import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { buildViewer } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails();

describe("When viewer opens a session", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then attendance cannot be marked", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
      { member: buildViewer() },
    );

    expect(await screen.findByText(session.classGroupName)).toBeOnTheScreen();
    expect(screen.queryByText(translate("sessions.attendance.swipeHint"))).toBeNull();
  });
});

import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [buildSessionStudent({ classBalance: null })],
});

describe("When a student pays a monthly fee", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then no class balance is shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(await screen.findByText("Tomás Pérez")).toBeOnTheScreen();
    expect(screen.queryByText(translate("sessions.attendance.noClassesLeft"))).toBeNull();
    expect(screen.queryByText(translate("sessions.attendance.oneClassLeft"))).toBeNull();
  });
});

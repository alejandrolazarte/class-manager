import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({ canTakeAttendance: false });
const student = buildSessionStudent();

describe("When session is in the future", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then attendance chips are disabled", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByRole("button", {
        name: translate("sessions.attendance.presentFor", { name: student.studentFullName }),
      }),
    ).toBeDisabled();
    expect(screen.getByText(translate("sessions.session.notYet"))).toBeOnTheScreen();
  });
});

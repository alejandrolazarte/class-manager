import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const student = buildSessionStudent({
  studentId: "student-1",
  studentFullName: "Tomás Pérez",
  status: "Present",
});
const session = buildSessionDetails({ isPast: true, students: [student] });

describe("When a past class is opened", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then attendance cannot be changed", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByRole("button", { name: translate("sessions.session.edit") }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: translate("sessions.attendance.presentFor", { name: student.studentFullName }),
      }),
    ).not.toBeOnTheScreen();
  });
});

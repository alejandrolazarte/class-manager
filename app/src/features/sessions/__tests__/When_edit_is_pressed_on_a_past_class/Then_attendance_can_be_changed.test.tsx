import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordAttendance } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const student = buildSessionStudent({
  studentId: "student-1",
  studentFullName: "Tomás Pérez",
  status: "Absent",
});
const session = buildSessionDetails({ isPast: true, students: [student] });

describe("When edit is pressed on a past class", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordAttendance).mockResolvedValue(undefined);
  });

  it("Then attendance can be changed", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.session.edit") }),
    );
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("sessions.attendance.presentFor", { name: student.studentFullName }),
      }),
    );

    await waitFor(() =>
      expect(recordAttendance).toHaveBeenCalledWith(
        session.classGroupId,
        sessionDate,
        student.studentId,
        "Present",
      ),
    );
  });
});

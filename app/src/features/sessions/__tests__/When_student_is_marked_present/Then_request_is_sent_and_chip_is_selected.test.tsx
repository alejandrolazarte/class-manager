import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordAttendance } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails();
const student = buildSessionStudent();

describe("When student is marked present", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordAttendance).mockResolvedValue(undefined);
  });

  it("Then request is sent and chip is selected", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    const presentChip = await screen.findByRole("button", {
      name: translate("sessions.attendance.presentFor", { name: student.studentFullName }),
    });

    await fireEvent.press(presentChip);

    await waitFor(() =>
      expect(recordAttendance).toHaveBeenCalledWith(
        session.classGroupId,
        sessionDate,
        student.studentId,
        "Present",
      ),
    );
    expect(presentChip).toBeSelected();
  });
});

import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordAttendance } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const serverErrorStatus = 500;
const session = buildSessionDetails();
const student = buildSessionStudent();

describe("When marking fails", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordAttendance).mockRejectedValue(new ApiError(serverErrorStatus, {}));
  });

  it("Then mark reverts and error is shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    const absentChip = await screen.findByRole("button", {
      name: translate("sessions.attendance.absentFor", { name: student.studentFullName }),
    });

    await fireEvent.press(absentChip);

    expect(await screen.findByText(translate("sessions.attendance.saveFailed"))).toBeOnTheScreen();
    expect(absentChip).not.toBeSelected();
  });
});

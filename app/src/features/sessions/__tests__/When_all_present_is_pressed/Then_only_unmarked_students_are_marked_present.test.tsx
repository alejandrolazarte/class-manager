import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordClassAttendance } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [
    buildSessionStudent({ studentId: "absent", studentFullName: "Lucía Pérez", status: "Absent" }),
    buildSessionStudent({ studentId: "unmarked", studentFullName: "Emma Suárez", status: null }),
  ],
});

describe("When all present is pressed", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordClassAttendance).mockResolvedValue(undefined);
  });

  it("Then only unmarked students are marked present", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.attendance.allPresent") }),
    );

    await waitFor(() =>
      expect(recordClassAttendance).toHaveBeenCalledWith(session.classGroupId, sessionDate, [
        { studentId: "unmarked", status: "Present" },
      ]),
    );
    expect(recordClassAttendance).toHaveBeenCalledTimes(1);
  });
});

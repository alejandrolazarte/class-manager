import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordAttendance } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [
    buildSessionStudent({
      studentId: "warned",
      studentFullName: "Tomás Pérez",
      absenceNotified: true,
    }),
    buildSessionStudent({ studentId: "coming", studentFullName: "Emma Suárez" }),
  ],
});

describe("When a student warned they won't come", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordAttendance).mockResolvedValue(undefined);
  });

  it("Then all present skips them", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(translate("sessions.attendance.absenceNotified")),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.attendance.allPresent") }),
    );

    await waitFor(() =>
      expect(recordAttendance).toHaveBeenCalledWith(
        session.classGroupId,
        sessionDate,
        "coming",
        "Present",
      ),
    );
    expect(recordAttendance).toHaveBeenCalledTimes(1);
  });
});

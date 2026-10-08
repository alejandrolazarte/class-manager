import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, recordFeedback } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const comment = "Hoy mantuvo la postura en todo el circuito.";
const session = buildSessionDetails({
  students: [buildSessionStudent({ studentId: "student-tomas", studentFullName: "Tomás Pérez" })],
});

describe("When instructor leaves a comment for a student", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(recordFeedback).mockResolvedValue();
  });

  it("Then the comment is sent", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("sessions.feedback.openFor", { name: "Tomás Pérez" }),
      }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("sessions.feedback.label")),
      comment,
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.feedback.save") }),
    );

    await waitFor(() =>
      expect(recordFeedback).toHaveBeenCalledWith(
        session.classGroupId,
        sessionDate,
        "student-tomas",
        comment,
      ),
    );
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession, removeSubstitute } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  instructorId: "0192f0d1-0000-7000-8000-000000000002",
  instructorFullName: "Marcos Díaz",
  originalInstructorFullName: "Laura Gómez",
});

describe("When session has a substitute", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(removeSubstitute).mockResolvedValue(undefined);
  });

  it("Then notice and remove are shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(
        translate("sessions.substitute.notice", {
          substitute: "Marcos Díaz",
          original: "Laura Gómez",
        }),
      ),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.substitute.remove") }),
    );
    await waitFor(() =>
      expect(removeSubstitute).toHaveBeenCalledWith(session.classGroupId, sessionDate),
    );
  });
});

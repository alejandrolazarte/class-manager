import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { assignSubstitute, getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/instructors/instructorsApi");

const session = buildSessionDetails();
const substitute = buildInstructor({
  id: "0192f0d1-0000-7000-8000-000000000002",
  fullName: "Marcos Díaz",
});

describe("When substitute is assigned", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor(), substitute]);
    jest.mocked(assignSubstitute).mockResolvedValue(undefined);
  });

  it("Then request has the instructor", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.substitute.open") }),
    );

    expect(screen.queryByRole("button", { name: session.instructorFullName })).toBeNull();
    await fireEvent.press(await screen.findByRole("button", { name: substitute.fullName }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.substitute.confirm") }),
    );

    await waitFor(() =>
      expect(assignSubstitute).toHaveBeenCalledWith(
        session.classGroupId,
        sessionDate,
        substitute.id,
      ),
    );
  });
});

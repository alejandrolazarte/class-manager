import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { assignSubstitute, getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/instructors/instructorsApi");

const conflictStatus = 409;
const session = buildSessionDetails();
const substitute = buildInstructor({
  id: "0192f0d1-0000-7000-8000-000000000002",
  fullName: "Marcos Díaz",
});

describe("When substitute is busy", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor(), substitute]);
    jest
      .mocked(assignSubstitute)
      .mockRejectedValue(new ApiError(conflictStatus, { code: sessionErrorCodes.instructorBusy }));
  });

  it("Then busy message is shown", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.substitute.open") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: substitute.fullName }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.substitute.confirm") }),
    );

    expect(
      await screen.findByText(translate("sessions.substitute.instructorBusy")),
    ).toBeOnTheScreen();
  });
});

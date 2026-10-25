import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [buildSessionStudent({ classBalance: { availableClasses: 0, unpaidClasses: 0 } })],
});

describe("When a student has no pack classes left", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then the row says there are no classes", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(translate("sessions.attendance.noClassesLeft")),
    ).toBeOnTheScreen();
  });
});

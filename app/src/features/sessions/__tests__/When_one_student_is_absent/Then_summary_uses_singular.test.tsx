import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [
    buildSessionStudent({
      studentId: "present",
      studentFullName: "Tomás Pérez",
      status: "Present",
    }),
    buildSessionStudent({ studentId: "absent", studentFullName: "Lucía Pérez", status: "Absent" }),
  ],
});

describe("When one student is absent", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then summary uses singular", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(await screen.findByLabelText("1 vino · 1 faltó · 0 sin marcar")).toBeOnTheScreen();
  });
});

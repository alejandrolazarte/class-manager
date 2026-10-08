import { screen } from "@testing-library/react-native";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const session = buildSessionDetails({
  students: [
    buildSessionStudent({
      studentFullName: "Tomás Pérez",
      clientFullName: "Ana Pérez",
      status: "Absent",
    }),
  ],
});

describe("When a child was marked absent", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
  });

  it("Then the status is shown next to the guardian", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(
        `${translate("sessions.attendance.absent")} · ${translate("students.list.responsible", { name: "Ana Pérez" })}`,
      ),
    ).toBeOnTheScreen();
  });
});

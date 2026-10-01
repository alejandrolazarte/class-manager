import { screen } from "@testing-library/react-native";
import { AgendaSessionCard } from "@/features/sessions/components/AgendaSessionCard";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession } from "@/testing/sessionFactory";

const session = buildDaySession({
  instructorFullName: "Marcos Díaz",
  originalInstructorFullName: "Laura Gómez",
});

describe("When day has a substituted class", () => {
  it("Then card shows who it replaces", async () => {
    await renderWithProviders(
      <AgendaSessionCard
        session={session}
        title={session.classGroupName}
        timing="upcoming"
        timeNow="10:00"
        isToday={false}
        onPress={jest.fn()}
      />,
    );

    expect(
      await screen.findByText(
        translate("sessions.day.substitute", {
          substitute: "Marcos Díaz",
          original: "Laura Gómez",
        }),
        { exact: false },
      ),
    ).toBeOnTheScreen();
  });
});

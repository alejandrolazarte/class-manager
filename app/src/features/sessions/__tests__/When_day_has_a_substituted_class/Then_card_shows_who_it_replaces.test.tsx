import { screen } from "@testing-library/react-native";
import { DaySessionCard } from "@/features/sessions/components/DaySessionCard";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession } from "@/testing/sessionFactory";

const session = buildDaySession({
  instructorFullName: "Marcos Díaz",
  originalInstructorFullName: "Laura Gómez",
});

describe("When day has a substituted class", () => {
  it("Then card shows who it replaces", async () => {
    await renderWithProviders(<DaySessionCard session={session} onPress={jest.fn()} />);

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

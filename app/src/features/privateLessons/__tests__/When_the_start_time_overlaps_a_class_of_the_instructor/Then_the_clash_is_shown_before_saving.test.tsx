import { fireEvent, screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { listInstructorAgenda } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInstructorAgendaSlot } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");

describe("When the start time overlaps a class of the instructor", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(listInstructorAgenda).mockResolvedValue([
      buildInstructorAgendaSlot({
        startTime: "09:00",
        durationMinutes: 60,
        classGroupName: "Grupo mañana",
      }),
    ]);
  });

  it("Then the clash is shown before saving", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("privateLessons.form.startTime")),
      "0930",
    );

    expect(
      await screen.findByText(
        translate("privateLessons.agenda.conflict", {
          start: "09:30",
          end: "10:15",
          name: "Grupo mañana",
          busyStart: "09:00",
          busyEnd: "10:00",
        }),
      ),
    ).toBeOnTheScreen();
  });
});

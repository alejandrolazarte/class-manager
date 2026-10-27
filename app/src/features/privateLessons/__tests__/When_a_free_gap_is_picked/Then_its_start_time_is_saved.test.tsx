import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import {
  listInstructorAgenda,
  schedulePrivateLesson,
} from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInstructorAgendaSlot, buildPrivateLesson } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const student = buildStudentSummary();

describe("When a free gap is picked", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest
      .mocked(listInstructorAgenda)
      .mockResolvedValue([buildInstructorAgendaSlot({ startTime: "09:00", durationMinutes: 60 })]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(schedulePrivateLesson).mockResolvedValue([buildPrivateLesson()]);
  });

  it("Then its start time is saved", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("privateLessons.form.chooseStudent") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("privateLessons.agenda.gapFrom", { time: "10:00" }),
      }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(schedulePrivateLesson).toHaveBeenCalledWith(
        expect.objectContaining({ startTime: "10:00" }),
      ),
    );
  });
});

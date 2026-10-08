import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { schedulePrivateLesson } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildPrivateLesson } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const firstInstructor = buildInstructor({ id: "instructor-first", fullName: "Marcos Díaz" });
const chosenInstructor = buildInstructor({ id: "instructor-chosen", fullName: "Laura Gómez" });
const student = buildStudentSummary();

describe("When private lesson has several instructors", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([firstInstructor, chosenInstructor]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(schedulePrivateLesson).mockResolvedValue([buildPrivateLesson()]);
  });

  it("Then the instructor chosen in the picker is sent", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("privateLessons.form.chooseStudent") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("instructors.picker.choose") }),
    );
    await fireEvent.press(
      within(screen.getByTestId("private-lesson-instructor-picker")).getByRole("button", {
        name: chosenInstructor.fullName,
      }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("privateLessons.form.startTime")),
      "1800",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(schedulePrivateLesson).toHaveBeenCalledWith(
        expect.objectContaining({ instructorId: chosenInstructor.id }),
      ),
    );
  });
});

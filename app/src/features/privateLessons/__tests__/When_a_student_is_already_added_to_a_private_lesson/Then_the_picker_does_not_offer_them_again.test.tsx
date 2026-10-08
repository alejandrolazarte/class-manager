import { fireEvent, screen, within } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const addedStudent = buildStudentSummary({ id: "student-added", fullName: "Tomás Pérez" });
const otherStudent = buildStudentSummary({ id: "student-other", fullName: "Lucía Gómez" });

describe("When a student is already added to a private lesson", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(searchStudents).mockResolvedValue([addedStudent, otherStudent]);
  });

  it("Then the picker does not offer them again", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("privateLessons.form.chooseStudent") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: addedStudent.fullName }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("privateLessons.form.addStudent") }),
    );

    const picker = within(screen.getByTestId("private-lesson-student-picker"));
    expect(await picker.findByRole("button", { name: otherStudent.fullName })).toBeTruthy();
    expect(picker.queryByRole("button", { name: addedStudent.fullName })).toBeNull();
  });
});

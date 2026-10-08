import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { schedulePrivateLesson } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildPrivateLesson } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const instructor = buildInstructor();
const student = buildStudentSummary();

describe("When private lesson is scheduled weekly", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(schedulePrivateLesson).mockResolvedValue([buildPrivateLesson()]);
  });

  it("Then request has students instructor and weeks", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("privateLessons.form.chooseStudent") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));
    await fireEvent.changeText(
      screen.getByLabelText(translate("privateLessons.form.startTime")),
      "1800",
    );
    for (let press = 0; press < 9; press += 1) {
      await fireEvent.press(
        screen.getByRole("button", { name: translate("privateLessons.form.moreWeeks") }),
      );
    }

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(schedulePrivateLesson).toHaveBeenCalledWith({
        instructorId: instructor.id,
        studentIds: [student.id],
        date: "2026-10-06",
        startTime: "18:00",
        durationMinutes: 45,
        location: null,
        notes: null,
        repeatWeeks: 10,
        isTrial: false,
        trialPrice: null,
      }),
    );
  });
});

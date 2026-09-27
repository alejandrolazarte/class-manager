import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { privateLessonErrorCodes } from "@/features/privateLessons/privateLessonErrorCodes";
import { schedulePrivateLesson } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { formatLongDate } from "@/features/sessions/dates";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const conflictStatus = 409;
const busyDate = "2026-10-20";
const student = buildStudentSummary();

describe("When coach is busy on a later week", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(schedulePrivateLesson).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: privateLessonErrorCodes.instructorBusy,
        date: busyDate,
      }),
    );
  });

  it("Then the conflicting date is shown", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("privateLessons.form.searchStudent")),
      "Tom",
    );
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));
    await fireEvent.changeText(
      screen.getByLabelText(translate("privateLessons.form.startTime")),
      "1800",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    expect(
      await screen.findByText(
        translate("privateLessons.coachBusy", { date: formatLongDate(busyDate) }),
      ),
    ).toBeOnTheScreen();
  });
});

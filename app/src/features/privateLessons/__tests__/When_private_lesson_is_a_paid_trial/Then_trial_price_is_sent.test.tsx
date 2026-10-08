import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { schedulePrivateLesson } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildPrivateLesson } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const student = buildStudentSummary();

describe("When private lesson is a paid trial", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance({ purchases: [] }));
    jest.mocked(schedulePrivateLesson).mockResolvedValue([buildPrivateLesson()]);
  });

  it("Then trial price is sent", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("privateLessons.form.chooseStudent") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));
    await fireEvent.changeText(
      screen.getByLabelText(translate("privateLessons.form.startTime")),
      "1800",
    );
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("privateLessons.form.isTrial") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("privateLessons.form.trialPrice")),
      "25",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(schedulePrivateLesson).toHaveBeenCalledWith(
        expect.objectContaining({ isTrial: true, trialPrice: 25 }),
      ),
    );
  });
});

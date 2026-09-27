import { fireEvent, screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildClassBalance } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/privateLessons/privateLessonsApi");
jest.mock("@/features/students/studentsApi");

const student = buildStudentSummary();
const courseBalance = buildClassBalance();
const coursePurchase = { ...courseBalance.purchases[0]!, classDurationMinutes: 30 };

describe("When student with a course pack is added", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest
      .mocked(getClassBalance)
      .mockResolvedValue({ ...courseBalance, purchases: [coursePurchase] });
  });

  it("Then duration comes from the pack", async () => {
    await renderWithProviders(<PrivateLessonFormScreen initialDate="2026-10-06" />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("privateLessons.form.searchStudent")),
      "Tom",
    );

    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));

    expect(await screen.findByDisplayValue("30", { exact: true })).toHaveProp(
      "accessibilityLabel",
      translate("classGroups.form.customDuration"),
    );
  });
});

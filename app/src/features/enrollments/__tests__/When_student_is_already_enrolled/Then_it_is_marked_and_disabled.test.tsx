import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentScreen } from "@/features/enrollments/screens/EnrollStudentScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/students/studentsApi");

const classGroup = buildClassGroup();
const student = buildStudentSummary();

describe("When student is already enrolled", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([buildRosterEntry({ studentId: student.id })]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
  });

  it("Then it is marked and disabled", async () => {
    await renderWithProviders(<EnrollStudentScreen classGroupId={classGroup.id} />);

    expect(
      await screen.findByText(translate("enrollments.enroll.alreadyEnrolled")),
    ).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: student.fullName })).toBeDisabled();
  });
});

import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentScreen } from "@/features/enrollments/screens/EnrollStudentScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { todayIsoDate } from "@/features/sessions/dates";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/students/studentsApi");

const classGroup = buildClassGroup();
const student = buildStudentSummary();

describe("When a student leaves the class today", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest
      .mocked(listClassRoster)
      .mockResolvedValue([buildRosterEntry({ studentId: student.id, endDate: todayIsoDate() })]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
  });

  it("Then they can be enrolled again", async () => {
    await renderWithProviders(<EnrollStudentScreen classGroupId={classGroup.id} />);

    expect(await screen.findByRole("button", { name: student.fullName })).toBeEnabled();
  });
});

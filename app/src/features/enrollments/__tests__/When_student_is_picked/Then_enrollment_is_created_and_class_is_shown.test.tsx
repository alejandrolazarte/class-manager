import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { enrollStudent, listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentScreen } from "@/features/enrollments/screens/EnrollStudentScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/students/studentsApi");

const classGroup = buildClassGroup();
const student = buildStudentSummary();

describe("When student is picked", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(enrollStudent).mockResolvedValue({
      id: "enrollment",
      classGroupId: classGroup.id,
      studentId: student.id,
      startDate: "2026-09-24",
      endDate: null,
    });
  });

  it("Then enrollment is created and class is shown", async () => {
    await renderWithProviders(<EnrollStudentScreen classGroupId={classGroup.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));

    await waitFor(() =>
      expect(enrollStudent).toHaveBeenCalledWith(classGroup.id, { studentId: student.id }),
    );
    await waitFor(() => expect(routerMock.back).toHaveBeenCalled());
  });
});

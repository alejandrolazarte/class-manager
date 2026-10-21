import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { enrollmentErrorCodes } from "@/features/enrollments/enrollmentErrorCodes";
import { enrollStudent, listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { EnrollStudentScreen } from "@/features/enrollments/screens/EnrollStudentScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/students/studentsApi");

const conflictStatus = 409;
const classGroup = buildClassGroup();
const student = buildStudentSummary();

describe("When enrolling outside the pack is confirmed", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest
      .mocked(enrollStudent)
      .mockRejectedValueOnce(
        new ApiError(conflictStatus, { code: enrollmentErrorCodes.outsidePack }),
      )
      .mockResolvedValue({
        id: "0192f0e1-0000-7000-8000-000000000001",
        classGroupId: classGroup.id,
        studentId: student.id,
        startDate: "2026-09-24",
        endDate: null,
      });
  });

  it("Then the student is enrolled", async () => {
    await renderWithProviders(<EnrollStudentScreen classGroupId={classGroup.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("enrollments.enroll.confirmOutsidePack"),
      }),
    );

    await waitFor(() =>
      expect(enrollStudent).toHaveBeenLastCalledWith(classGroup.id, {
        studentId: student.id,
        allowOutsidePack: true,
      }),
    );
  });
});

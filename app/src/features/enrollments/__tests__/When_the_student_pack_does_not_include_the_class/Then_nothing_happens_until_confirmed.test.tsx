import { fireEvent, screen } from "@testing-library/react-native";
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

describe("When the student pack does not include the class", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest
      .mocked(enrollStudent)
      .mockRejectedValue(new ApiError(conflictStatus, { code: enrollmentErrorCodes.outsidePack }));
  });

  it("Then nothing happens until confirmed", async () => {
    await renderWithProviders(<EnrollStudentScreen classGroupId={classGroup.id} />);

    await fireEvent.press(await screen.findByRole("button", { name: student.fullName }));

    expect(
      await screen.findByText(
        translate("enrollments.enroll.outsidePackQuestion", { student: student.fullName }),
      ),
    ).toBeOnTheScreen();
    expect(enrollStudent).toHaveBeenCalledTimes(1);
    expect(enrollStudent).not.toHaveBeenCalledWith(
      classGroup.id,
      expect.objectContaining({ allowOutsidePack: true }),
    );
  });
});

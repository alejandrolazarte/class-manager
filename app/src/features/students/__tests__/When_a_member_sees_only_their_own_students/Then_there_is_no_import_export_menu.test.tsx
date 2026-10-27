import { screen } from "@testing-library/react-native";
import { permissions } from "@/features/members/permissions";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildInstructorMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/students/studentsApi");

describe("When a member sees only their own students", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([buildStudentSummary()]);
  });

  it("Then there is no import export menu", async () => {
    const instructor = buildInstructorMember();
    await renderWithProviders(<StudentListScreen />, {
      member: {
        ...instructor,
        permissions: [...instructor.permissions, permissions.importExportRun],
      },
    });

    expect(await screen.findByText(translate("students.list.title"))).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("importExport.moreOptions") }),
    ).toBeNull();
  });
});

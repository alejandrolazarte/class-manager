import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/students/studentsApi");

describe("When a member can import and export", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([buildStudentSummary()]);
  });

  it("Then the students list offers the menu", async () => {
    await renderWithProviders(<StudentListScreen />);

    expect(
      await screen.findByRole("button", { name: translate("importExport.moreOptions") }),
    ).toBeOnTheScreen();
  });
});

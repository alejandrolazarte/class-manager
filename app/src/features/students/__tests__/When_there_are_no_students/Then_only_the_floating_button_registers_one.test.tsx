import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/students/studentsApi");

describe("When there are no students", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then only the floating button registers one", async () => {
    await renderWithProviders(<StudentListScreen />);

    expect(await screen.findByText(translate("students.list.emptyTitle"))).toBeOnTheScreen();
    expect(
      screen.getAllByRole("button", { name: translate("students.list.newStudent") }),
    ).toHaveLength(1);
    expect(
      screen.getByText(
        translate("common.emptyStateHint", { action: translate("students.list.newStudent") }),
      ),
    ).toBeOnTheScreen();
  });
});

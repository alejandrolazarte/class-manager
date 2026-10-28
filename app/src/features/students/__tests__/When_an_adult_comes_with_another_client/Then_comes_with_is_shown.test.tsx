import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/students/studentsApi");

describe("When an adult comes with another client", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([
      buildStudentSummary({
        fullName: "Lucía Gómez",
        birthDate: "1995-04-10",
        clientFullName: "Martín Ruiz",
      }),
    ]);
  });

  it("Then comes with is shown", async () => {
    await renderWithProviders(<StudentListScreen />);

    expect(
      await screen.findByText(translate("students.list.comesWith", { name: "Martín Ruiz" })),
    ).toBeOnTheScreen();
  });
});

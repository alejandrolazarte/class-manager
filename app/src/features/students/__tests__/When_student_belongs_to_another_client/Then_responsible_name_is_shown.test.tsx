import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/students/studentsApi");

describe("When student belongs to another client", () => {
  beforeEach(() => {
    jest
      .mocked(searchStudents)
      .mockResolvedValue([
        buildStudentSummary({ fullName: "Tomás Pérez", clientFullName: "Ana Pérez" }),
        buildStudentSummary({ id: "adult", fullName: "Ana Pérez", clientFullName: "Ana Pérez" }),
      ]);
  });

  it("Then responsible name is shown", async () => {
    await renderWithProviders(<StudentListScreen />);

    expect(
      await screen.findAllByText(translate("students.list.responsible", { name: "Ana Pérez" })),
    ).toHaveLength(1);
  });
});

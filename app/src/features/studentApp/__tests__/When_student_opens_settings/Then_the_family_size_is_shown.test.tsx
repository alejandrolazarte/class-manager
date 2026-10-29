import { screen } from "@testing-library/react-native";
import { getStudentAppFamily, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translateCount } from "@/i18n/translate";
import { buildStudentAppFamily, buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens settings", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppFamily).mockResolvedValue(buildStudentAppFamily());
  });

  it("Then the family size is shown", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    expect(await screen.findByText(translateCount("student.family.count", 2))).toBeOnTheScreen();
  });
});

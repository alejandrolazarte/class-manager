import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassScreen } from "@/features/studentApp/screens/StudentAppClassScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When the class detail loses its route parameters", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
  });

  it("Then it still renders", async () => {
    await renderStudentAppScreen(<StudentAppClassScreen />);

    expect(await screen.findByText(translate("student.tabs.classes"))).toBeOnTheScreen();
  });
});

import { screen } from "@testing-library/react-native";
import { getStudentAppFamily } from "@/features/studentApp/studentAppApi";
import { StudentAppFamilyScreen } from "@/features/studentApp/screens/StudentAppFamilyScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppFamily } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the family", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppFamily).mockResolvedValue(buildStudentAppFamily());
  });

  it("Then the responsible is marked", async () => {
    await renderStudentAppScreen(<StudentAppFamilyScreen />);

    expect(await screen.findByText(translate("student.family.responsible"))).toBeOnTheScreen();
  });
});

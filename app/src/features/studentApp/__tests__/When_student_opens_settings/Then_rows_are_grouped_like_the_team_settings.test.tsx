import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const student = buildStudentAppHome();

describe("When student opens settings", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(student);
  });

  it("Then rows are grouped like the team settings", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);
    await screen.findByText(student.clientFullName);

    expect(
      screen.getByRole("button", { name: translate("settings.appearance") }),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("settings.group.device"))).toBeOnTheScreen();
    expect(screen.getByText(translate("settings.group.account"))).toBeOnTheScreen();
  });
});

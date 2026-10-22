import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const student = buildStudentAppHome({ location: null });

describe("When the business has no location", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(student);
  });

  it("Then settings have no location group", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);
    await screen.findByText(student.clientFullName);

    expect(screen.queryByText(translate("settings.group.location"))).toBeNull();
  });
});

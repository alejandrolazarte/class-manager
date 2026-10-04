import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getPushKey } from "@/features/studentApp/studentAppApi";
import { browserPushSupport } from "@/features/studentApp/push/browserPush";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");

const student = buildStudentAppHome();

describe("When notifications cannot be offered", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(student);
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "" });
    jest.mocked(browserPushSupport).mockReturnValue("supported");
  });

  it("Then no empty card is shown", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);
    await screen.findByText(student.clientFullName);

    expect(screen.queryByText(translate("student.notifications.title"))).toBeNull();
  });
});

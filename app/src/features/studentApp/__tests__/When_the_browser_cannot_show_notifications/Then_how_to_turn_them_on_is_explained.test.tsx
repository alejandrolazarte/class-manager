import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome, getPushKey } from "@/features/studentApp/studentAppApi";
import { browserPushSupport } from "@/features/studentApp/push/browserPush";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");

describe("When the browser cannot show notifications", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(browserPushSupport).mockReturnValue("unsupported");
  });

  it("Then how to turn them on is explained", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.notifications.title") }),
    );

    expect(
      await screen.findByText(translate("student.notifications.unsupported")),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("switch", { name: translate("student.notifications.toggle") }),
    ).toBeNull();
  });
});

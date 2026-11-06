import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome, getPushKey } from "@/features/studentApp/studentAppApi";
import { browserPushSupport } from "@/features/studentApp/push/browserPush";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");

describe("When notifications is pressed in settings", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "" });
    jest.mocked(browserPushSupport).mockReturnValue("unsupported");
  });

  it("Then the notifications screen opens", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.notifications.title") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.studentAppNotificationPreferences);
  });
});

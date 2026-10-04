import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentAppHome,
  getPushKey,
  removePushSubscription,
} from "@/features/studentApp/studentAppApi";
import {
  browserPushSupport,
  currentBrowserSubscription,
  unsubscribeBrowser,
} from "@/features/studentApp/push/browserPush";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");

const subscription = {
  endpoint: "https://push.example.com/send/1",
  p256dh: "public-key",
  auth: "auth-secret",
};

describe("When student turns off notifications", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(removePushSubscription).mockResolvedValue();
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest.mocked(currentBrowserSubscription).mockResolvedValue(subscription);
    jest.mocked(unsubscribeBrowser).mockResolvedValue(subscription.endpoint);
  });

  it("Then the subscription is removed", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.notifications.title") }),
    );

    const toggle = await screen.findByRole("switch", {
      name: translate("student.notifications.toggle"),
    });
    await waitFor(() => expect(toggle).toBeChecked());
    await fireEvent.press(toggle);

    await waitFor(() => expect(removePushSubscription).toHaveBeenCalledWith(subscription.endpoint));
  });
});

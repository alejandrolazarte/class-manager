import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentAppHome,
  getPushKey,
  savePushSubscription,
} from "@/features/studentApp/studentAppApi";
import {
  browserPushSupport,
  currentBrowserSubscription,
  subscribeBrowser,
} from "@/features/studentApp/push/browserPush";
import { StudentAppNotificationPreferencesScreen } from "@/features/studentApp/screens/StudentAppNotificationPreferencesScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");
jest.mock("@/features/notifications/notificationPreferencesApi");

const subscription = {
  endpoint: "https://push.example.com/send/1",
  p256dh: "public-key",
  auth: "auth-secret",
};

describe("When student turns on notifications", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(savePushSubscription).mockResolvedValue();
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest.mocked(currentBrowserSubscription).mockResolvedValue(null);
    jest.mocked(subscribeBrowser).mockResolvedValue(subscription);
  });

  it("Then the subscription is saved", async () => {
    await renderStudentAppScreen(<StudentAppNotificationPreferencesScreen />);

    await fireEvent.press(
      await screen.findByRole("switch", { name: translate("student.notifications.toggle") }),
    );

    await waitFor(() => expect(savePushSubscription).toHaveBeenCalledWith(subscription));
    expect(subscribeBrowser).toHaveBeenCalledWith("vapid-public-key");
  });
});

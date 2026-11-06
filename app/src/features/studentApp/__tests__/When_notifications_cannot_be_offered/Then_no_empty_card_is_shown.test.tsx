import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getPushKey } from "@/features/studentApp/studentAppApi";
import { browserPushSupport } from "@/features/studentApp/push/browserPush";
import { StudentAppNotificationPreferencesScreen } from "@/features/studentApp/screens/StudentAppNotificationPreferencesScreen";
import { getMyNotificationPreferences } from "@/features/notifications/notificationPreferencesApi";
import {
  buildNotificationPreference,
  buildNotificationPreferences,
} from "@/testing/notificationPreferenceFactory";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/push/browserPush");
jest.mock("@/features/notifications/notificationPreferencesApi");

const student = buildStudentAppHome();

describe("When notifications cannot be offered", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(student);
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "" });
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest
      .mocked(getMyNotificationPreferences)
      .mockResolvedValue(
        buildNotificationPreferences([buildNotificationPreference("Orders", "Family")]),
      );
  });

  it("Then no empty card is shown", async () => {
    await renderStudentAppScreen(<StudentAppNotificationPreferencesScreen />);
    await screen.findByText(translate("notificationPreferences.category.Orders"));

    expect(screen.queryByText(translate("notificationPreferences.device"))).toBeNull();
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyHome, getPushKey, removePushSubscription } from "@/features/family/familyApi";
import {
  browserPushSupport,
  currentBrowserSubscription,
  unsubscribeBrowser,
} from "@/features/family/push/browserPush";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");
jest.mock("@/features/family/push/browserPush");

const subscription = {
  endpoint: "https://push.example.com/send/1",
  p256dh: "public-key",
  auth: "auth-secret",
};

describe("When family turns off notifications", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(removePushSubscription).mockResolvedValue();
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest.mocked(currentBrowserSubscription).mockResolvedValue(subscription);
    jest.mocked(unsubscribeBrowser).mockResolvedValue(subscription.endpoint);
  });

  it("Then the subscription is removed", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);

    const toggle = await screen.findByRole("switch", {
      name: translate("family.notifications.toggle"),
    });
    await waitFor(() => expect(toggle).toBeChecked());
    await fireEvent.press(toggle);

    await waitFor(() => expect(removePushSubscription).toHaveBeenCalledWith(subscription.endpoint));
  });
});

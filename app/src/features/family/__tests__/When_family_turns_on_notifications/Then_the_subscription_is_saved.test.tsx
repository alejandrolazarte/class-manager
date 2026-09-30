import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyHome, getPushKey, savePushSubscription } from "@/features/family/familyApi";
import {
  browserPushSupport,
  currentBrowserSubscription,
  subscribeBrowser,
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

describe("When family turns on notifications", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(savePushSubscription).mockResolvedValue();
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest.mocked(currentBrowserSubscription).mockResolvedValue(null);
    jest.mocked(subscribeBrowser).mockResolvedValue(subscription);
  });

  it("Then the subscription is saved", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("switch", { name: translate("family.notifications.toggle") }),
    );

    await waitFor(() => expect(savePushSubscription).toHaveBeenCalledWith(subscription));
    expect(subscribeBrowser).toHaveBeenCalledWith("vapid-public-key");
  });
});

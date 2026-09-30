import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import {
  browserPushSupport,
  currentBrowserSubscription,
  subscribeBrowser,
} from "@/features/family/push/browserPush";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import {
  getTeamPushKey,
  saveTeamPushSubscription,
} from "@/features/teamNotifications/teamNotificationsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/family/push/browserPush");
jest.mock("@/features/teamNotifications/teamNotificationsApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

const subscription = { endpoint: "https://push.example.com/1", p256dh: "key", auth: "auth" };

describe("When coach turns on notifications", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
    jest.mocked(browserPushSupport).mockReturnValue("supported");
    jest.mocked(currentBrowserSubscription).mockResolvedValue(null);
    jest.mocked(subscribeBrowser).mockResolvedValue(subscription);
    jest.mocked(getTeamPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(saveTeamPushSubscription).mockResolvedValue();
  });

  it("Then this device is subscribed", async () => {
    await renderWithProviders(<SettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("switch", { name: translate("family.notifications.toggle") }),
    );

    await waitFor(() => expect(saveTeamPushSubscription).toHaveBeenCalledWith(subscription));
  });
});

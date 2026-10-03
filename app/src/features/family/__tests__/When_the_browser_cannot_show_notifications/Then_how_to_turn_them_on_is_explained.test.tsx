import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyHome, getPushKey } from "@/features/family/familyApi";
import { browserPushSupport } from "@/features/family/push/browserPush";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");
jest.mock("@/features/family/push/browserPush");

describe("When the browser cannot show notifications", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "vapid-public-key" });
    jest.mocked(browserPushSupport).mockReturnValue("unsupported");
  });

  it("Then how to turn them on is explained", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.notifications.title") }),
    );

    expect(
      await screen.findByText(translate("family.notifications.unsupported")),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("switch", { name: translate("family.notifications.toggle") }),
    ).toBeNull();
  });
});

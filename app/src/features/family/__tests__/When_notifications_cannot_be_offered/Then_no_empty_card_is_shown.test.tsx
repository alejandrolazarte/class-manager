import { screen } from "@testing-library/react-native";
import { getFamilyHome, getPushKey } from "@/features/family/familyApi";
import { browserPushSupport } from "@/features/family/push/browserPush";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");
jest.mock("@/features/family/push/browserPush");

const family = buildFamilyHome();

describe("When notifications cannot be offered", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(family);
    jest.mocked(getPushKey).mockResolvedValue({ publicKey: "" });
    jest.mocked(browserPushSupport).mockReturnValue("supported");
  });

  it("Then no empty card is shown", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);
    await screen.findByText(family.clientFullName);

    expect(screen.queryByTestId("notification-settings")).toBeNull();
    expect(screen.queryByText(translate("family.notifications.title"))).toBeNull();
  });
});

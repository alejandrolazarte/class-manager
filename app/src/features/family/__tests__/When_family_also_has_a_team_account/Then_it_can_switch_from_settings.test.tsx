import { fireEvent, screen } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildAccount } from "@/testing/accountFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/family/familyApi");

describe("When family also has a team account", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest
      .mocked(listAccounts)
      .mockResolvedValue([
        buildAccount({ isCurrent: false }),
        buildAccount({ businessId: "business-school", kind: "family", isCurrent: true }),
      ]);
  });

  it("Then it can switch from settings", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("accounts.switch") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.chooseAccount);
  });
});

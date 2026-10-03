import { screen } from "@testing-library/react-native";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilySettingsScreen } from "@/features/family/screens/FamilySettingsScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const family = buildFamilyHome();

describe("When family opens settings", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(family);
  });

  it("Then rows are grouped like the team settings", async () => {
    await renderFamilyScreen(<FamilySettingsScreen />);
    await screen.findByText(family.clientFullName);

    expect(
      screen.getByRole("button", { name: translate("settings.appearance") }),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("settings.group.device"))).toBeOnTheScreen();
    expect(screen.getByText(translate("settings.group.account"))).toBeOnTheScreen();
  });
});

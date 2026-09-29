import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyShop, buildFamilyHome, buildFamilyStudent } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family has no classes", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest
      .mocked(getFamilyHome)
      .mockResolvedValue(buildFamilyHome({ students: [buildFamilyStudent({ nextClasses: [] })] }));
  });

  it("Then empty message is shown", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(await screen.findByText(translate("family.student.noClasses"))).toBeOnTheScreen();
  });
});

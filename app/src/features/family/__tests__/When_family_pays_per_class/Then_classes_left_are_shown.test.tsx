import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translateCount } from "@/i18n/translate";
import { buildFamilyShop, buildFamilyHome } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family pays per class", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        billing: {
          kind: "ClassPacks",
          monthlyFee: null,
          classes: { availableClasses: 5, unpaidClasses: 0 },
        },
      }),
    );
  });

  it("Then classes left are shown", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(
      await screen.findByText(translateCount("family.classes.available", 5)),
    ).toBeOnTheScreen();
  });
});

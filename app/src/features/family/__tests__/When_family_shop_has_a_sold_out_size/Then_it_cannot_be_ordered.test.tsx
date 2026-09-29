import { screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/family/familyApi");

describe("When family shop has a sold out size", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
  });

  it("Then it cannot be ordered", async () => {
    await renderWithSession(<FamilyShopScreen />);

    expect(
      await screen.findByText(`Gorro de natación · M · ${translate("family.shop.soldOut")}`),
    ).toBeOnTheScreen();
    expect(
      screen.queryByLabelText(
        translate("orders.counterSale.increase", { name: "Gorro de natación · M" }),
      ),
    ).toBeNull();
  });
});

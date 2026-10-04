import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family removes the last unit from the cart", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
  });

  it("Then the cart is empty", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("family.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("family.cart.remove", { name: "8 clases" }) }),
    );

    expect(screen.getByText(translate("family.cart.empty"))).toBeOnTheScreen();
  });
});

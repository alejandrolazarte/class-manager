import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyShop, placeFamilyOrder } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildFamilyOrder, buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family orders from the shop", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(placeFamilyOrder).mockResolvedValue(buildFamilyOrder());
  });

  it("Then the pack and units are sent", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("family.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("family.shop.addToCart", { total: formatMoney(12, "EUR") }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeFamilyOrder).toHaveBeenCalledWith(
        [
          { classPackId: "pack-8", productVariantId: null, quantity: 1 },
          { classPackId: null, productVariantId: "variant-s", quantity: 1 },
        ],
        { delivery: "Pickup", deliveryClassGroupId: null },
      ),
    );
    expect(await screen.findByText(translate("family.cart.ordered"))).toBeOnTheScreen();
  });
});

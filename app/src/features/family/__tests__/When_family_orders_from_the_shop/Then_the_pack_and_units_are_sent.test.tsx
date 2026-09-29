import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyShop, placeFamilyOrder } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyOrder, buildFamilyShop } from "@/testing/familyFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/family/familyApi");

describe("When family orders from the shop", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(placeFamilyOrder).mockResolvedValue(buildFamilyOrder());
  });

  it("Then the pack and units are sent", async () => {
    await renderWithSession(<FamilyShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("family.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(
      screen.getByLabelText(
        translate("orders.counterSale.increase", { name: "Gorro de natación · S" }),
      ),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeFamilyOrder).toHaveBeenCalledWith([
        { classPackId: "pack-8", productVariantId: null, quantity: 1 },
        { classPackId: null, productVariantId: "variant-s", quantity: 1 },
      ]),
    );
  });
});

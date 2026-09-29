import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyShop, placeFamilyOrder } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildFamilyOrder, buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family asks for delivery in class", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(placeFamilyOrder).mockResolvedValue(buildFamilyOrder());
  });

  it("Then the class is sent", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("family.shop.increase") }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("family.shop.addToCart", { total: formatMoney(24, "EUR") }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(
      screen.getByRole("radio", {
        name: translate("delivery.inClass", {
          className: "Natación inicial",
          student: "Tomás Pérez",
        }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeFamilyOrder).toHaveBeenCalledWith(
        [{ classPackId: null, productVariantId: "variant-s", quantity: 2 }],
        { delivery: "InClass", deliveryClassGroupId: "class-group-1" },
      ),
    );
  });
});

import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyShop, placeFamilyOrder } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyOrder, buildFamilyShop } from "@/testing/familyFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/family/familyApi");

describe("When family asks for delivery in class", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(placeFamilyOrder).mockResolvedValue(buildFamilyOrder());
  });

  it("Then the class is sent", async () => {
    await renderWithSession(<FamilyShopScreen />);

    await fireEvent.press(
      await screen.findByLabelText(
        translate("orders.counterSale.increase", { name: "Gorro de natación · S" }),
      ),
    );
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("delivery.inClass", {
          className: "Natación inicial",
          student: "Tomás Pérez",
        }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeFamilyOrder).toHaveBeenCalledWith(
        [{ classPackId: null, productVariantId: "variant-s", quantity: 1 }],
        { delivery: "InClass", deliveryClassGroupId: "class-group-1" },
      ),
    );
  });
});

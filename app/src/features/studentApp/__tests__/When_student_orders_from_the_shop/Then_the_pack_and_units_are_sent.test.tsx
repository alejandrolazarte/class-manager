import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getStudentAppShop, placeStudentAppOrder } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildStudentAppOrder, buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student orders from the shop", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(placeStudentAppOrder).mockResolvedValue(buildStudentAppOrder());
  });

  it("Then the pack and units are sent", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("student.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("student.shop.addToCart", { total: formatMoney(12, "EUR") }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeStudentAppOrder).toHaveBeenCalledWith(
        [
          { classPackId: "pack-8", productVariantId: null, quantity: 1 },
          { classPackId: null, productVariantId: "variant-s", quantity: 1 },
        ],
        { delivery: "Pickup", deliveryClassGroupId: null },
      ),
    );
    expect(await screen.findByText(translate("student.cart.ordered"))).toBeOnTheScreen();
  });
});

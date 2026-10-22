import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getStudentAppShop, placeStudentAppOrder } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildStudentAppOrder, buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const addOneCap = async () => {
  await fireEvent.press(screen.getByRole("button", { name: "Gorro de natación" }));
  await fireEvent.press(
    screen.getByRole("button", {
      name: translate("student.shop.addToCart", { total: formatMoney(12, "EUR") }),
    }),
  );
};

describe("When student adds more units than are left", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(placeStudentAppOrder).mockResolvedValue(buildStudentAppOrder());
  });

  it("Then the cart keeps the units left", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);
    await screen.findByRole("button", { name: "Gorro de natación" });

    await addOneCap();
    await addOneCap();
    await addOneCap();
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(screen.getByRole("button", { name: /Pedir/ }));

    await waitFor(() =>
      expect(placeStudentAppOrder).toHaveBeenCalledWith(
        [{ classPackId: null, productVariantId: "variant-s", quantity: 2 }],
        { delivery: "Pickup", deliveryClassGroupId: null },
      ),
    );
  });
});

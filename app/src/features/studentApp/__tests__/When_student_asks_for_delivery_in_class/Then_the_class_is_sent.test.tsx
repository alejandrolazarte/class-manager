import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getStudentAppShop, placeStudentAppOrder } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildStudentAppOrder, buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student asks for delivery in class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(placeStudentAppOrder).mockResolvedValue(buildStudentAppOrder());
  });

  it("Then the class is sent", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("student.shop.increase") }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("student.shop.addToCart", { total: formatMoney(24, "EUR") }),
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
      expect(placeStudentAppOrder).toHaveBeenCalledWith(
        [{ classPackId: null, productVariantId: "variant-s", quantity: 2 }],
        { delivery: "InClass", deliveryClassGroupId: "class-group-1" },
      ),
    );
  });
});

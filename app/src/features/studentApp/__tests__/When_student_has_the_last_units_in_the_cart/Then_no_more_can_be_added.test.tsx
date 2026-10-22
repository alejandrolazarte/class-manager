import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has the last units in the cart", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
  });

  it("Then no more can be added", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("student.shop.increase") }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("student.shop.addToCart", { total: formatMoney(24, "EUR") }),
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));

    expect(
      screen.getByRole("button", {
        name: translate("student.cart.increase", { name: "Gorro de natación · S" }),
      }),
    ).toBeDisabled();
  });
});

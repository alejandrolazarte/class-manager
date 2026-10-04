import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student removes the last unit from the cart", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
  });

  it("Then the cart is empty", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("student.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("student.cart.remove", { name: "8 clases" }) }),
    );

    expect(screen.getByText(translate("student.cart.empty"))).toBeOnTheScreen();
  });
});

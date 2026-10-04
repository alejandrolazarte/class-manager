import { fireEvent, screen } from "@testing-library/react-native";
import { NetworkError } from "@/api/httpClient";
import { getStudentAppShop, placeStudentAppOrder } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When the order gets no answer", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(placeStudentAppOrder).mockRejectedValue(new NetworkError(new Error("timeout")));
  });

  it("Then it asks to check my orders", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: `${translate("student.shop.addPack")} 8 clases` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: /Ver carrito/ }));
    await fireEvent.press(screen.getByRole("button", { name: /^Pedir/ }));

    expect(await screen.findByText(translate("student.shop.noAnswer"))).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("student.cart.seeOrders") }),
    ).toBeOnTheScreen();
  });
});

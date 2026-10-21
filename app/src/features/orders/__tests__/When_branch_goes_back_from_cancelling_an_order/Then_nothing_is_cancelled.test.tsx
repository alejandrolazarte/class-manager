import { fireEvent, screen } from "@testing-library/react-native";
import { cancelOrder, listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When branch goes back from cancelling an order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder()]);
  });

  it("Then nothing is cancelled", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));

    const backButtons = screen.getAllByRole("button", { name: translate("common.back") });
    await fireEvent.press(backButtons[backButtons.length - 1]);

    expect(screen.queryByText(translate("orders.cancelConfirm.title"))).toBeNull();
    expect(cancelOrder).not.toHaveBeenCalled();
  });
});

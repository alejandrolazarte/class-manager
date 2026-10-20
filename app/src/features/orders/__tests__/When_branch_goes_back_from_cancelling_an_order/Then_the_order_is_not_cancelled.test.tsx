import { fireEvent, screen } from "@testing-library/react-native";
import { cancelOrder, listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const requestedOrder = buildOrder({
  channel: "App",
  status: "Requested",
  awaitsPickup: false,
  method: null,
  paidOn: null,
  clientFullName: "Ana Pérez",
});

describe("When branch goes back from cancelling an order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([requestedOrder]);
  });

  it("Then the order is not cancelled", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.collect") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.cancel") }));
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("common.back") }).at(-1)!,
    );

    expect(screen.queryByText(translate("orders.cancelMessage"))).not.toBeOnTheScreen();
    expect(cancelOrder).not.toHaveBeenCalled();
  });
});

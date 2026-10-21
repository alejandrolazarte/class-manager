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

describe("When branch presses cancel order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([requestedOrder]);
  });

  it("Then a confirm dialog opens without cancelling", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.collect") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(
        translate("orders.cancelQuestionWithClient", {
          number: requestedOrder.number,
          client: "Ana Pérez",
        }),
      ),
    ).toBeOnTheScreen();
    expect(cancelOrder).not.toHaveBeenCalled();
  });
});

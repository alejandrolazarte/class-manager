import { fireEvent, screen } from "@testing-library/react-native";
import { cancelOrder, listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const requestedOrder = buildOrder({
  channel: "App",
  paymentStatus: "Unpaid",
  lastPaidOn: null,
  paidAmount: 0,
  balance: 24,
  payments: [],
  clientFullName: "Ana Pérez",
});

describe("When branch cancels an order", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([requestedOrder]);
    jest.mocked(cancelOrder).mockResolvedValue(buildOrder({ status: "Cancelled" }));
  });

  it("Then order cancelled is shown", async () => {
    await renderWithProviders(<OrderListScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("orders.cancel") }).at(-1)!,
    );

    expect(await screen.findByText(translate("orders.cancelled"))).toBeOnTheScreen();
  });
});
